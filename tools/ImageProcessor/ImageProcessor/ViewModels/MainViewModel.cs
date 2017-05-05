using Microsoft.OneDrive.Sdk;
using Microsoft.OneDrive.Sdk.Authentication;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace ImageProcessor.ViewModels
{
    public class MainViewModel : DispatcherObservableObject
    {
        #region Private fields
        private string _baseFolder;
        private Visibility _progressVisibility = Visibility.Collapsed;
        private Visibility _signButtonVisibility = Visibility.Visible;
        private Visibility _formVisibility = Visibility.Collapsed;
        private ImageSource _imageOriginal;
        private ImageSource _imageCropped;
        private IEnumerable<Crop> _crops;
        private Crop _selectedCrop;
        private IEnumerable<Game> _games;
        private OneDriveClient _client;
        private Game _selectedGame;
        private Pack _selectedPack;
        private Section _selectedSection;
        private Level _selectedLevel;
        private int _currentIndex;
        private IEnumerable<Item> _items;
        private Item _rootFolder;
        private Item _othersFolder;
        private Item _handledFolder;
        private bool _isNextEnabled;
        private bool _isPreviousEnabled;
        private readonly string _clientId;
        //private const string _screenShotsFolderPath = "Pictures/Screenshots";
        private const string _handledFolderPath = "Pictures/Screenshots/Handled";
        private const string _otherFolderPath = "Pictures/Screenshots/Other";
        private const string _dataFolder = "data";
        private const string _targetFolder = "images";
        private Bitmap _croppedImage;
        private JsonSerializerSettings _serializerSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            Formatting = Formatting.Indented
        };
        #endregion

        public MainViewModel(string clientId, Dispatcher dispatcher) : base(dispatcher)
        {
            _clientId = clientId;
            _crops = new List<Crop>
            {
                new Crop
                {
                    Name = "Normal",
                    Start = 364
                },
                new Crop
                {
                    Name = "Application Bar",
                    Start = 322
                }
            };
            _selectedCrop = _crops.ElementAt(0);
        }

        #region Commands
        public ICommand SignIn => new ActionCommand("", async p =>
            await DoWithProgress(async () =>
            {
                SignButtonVisibility = Visibility.Collapsed;

                try
                {
                    await Authenticate().ConfigureAwait(false);
                    _rootFolder = SelectRootFolder();
                    _baseFolder = SelectFolder("Select Base Folder");
                    LoadData();
                    await LoadDriveComponents().ConfigureAwait(false);

                    FormVisibility = Visibility.Visible;
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    SignButtonVisibility = Visibility.Visible;
                }
            }).ConfigureAwait(false));

        public ICommand Next => new ActionCommand("", async p =>
            await DoWithProgress(async () =>
            {
                if (_currentIndex < _items?.Count())
                {
                    _currentIndex++;
                    await ShowImage().ConfigureAwait(false);
                    IsNextEnabled = _currentIndex < _items?.Count() - 1;
                    IsPreviousEnabled = true;
                };
            }).ConfigureAwait(false));

        public ICommand Previous => new ActionCommand("", async p =>
            await DoWithProgress(async () =>
            {
                if (_currentIndex != 0)
                {
                    _currentIndex--;
                    await ShowImage().ConfigureAwait(false);
                    IsPreviousEnabled = _currentIndex > 0;
                }
            }).ConfigureAwait(false));

        public ICommand MoveToOther => new ActionCommand("", async p =>
            await DoWithProgress(async () =>
            {
                if (null == _othersFolder) return;

                var updateItem = new Item
                {
                    ParentReference = new ItemReference
                    {
                        Id = _othersFolder.Id
                    }
                };

                await _client.Drive.Items[_items.ElementAt(_currentIndex).Id].Request().UpdateAsync(updateItem).ConfigureAwait(false);
                await LoadDriveComponents().ConfigureAwait(false);
            }).ConfigureAwait(false));

        public ICommand Save => new ActionCommand("", async p =>
        {
            if (null == SelectedGame
                || null == SelectedPack
                || null == SelectedSection
                || null == SelectedLevel
                || !SelectedLevel.Flows.HasValue)
                DispatcherInvoke(() => System.Windows.MessageBox.Show("Input not complete!", "Error", MessageBoxButton.OK, MessageBoxImage.Error));
            else
                await DoWithProgress(async () =>
                {
                    var targetFolder = Path.Combine(_baseFolder, _targetFolder, SelectedGame.Id, SelectedPack.Name, SelectedSection.Name);
                    Directory.CreateDirectory(targetFolder);
                    var targetPath = Path.Combine(targetFolder, $"{SelectedLevel.Number.ToString("D3")}.jpg");
                    _croppedImage.Save(targetPath, ImageFormat.Jpeg);

                    SelectedLevel.IsSolved = true;
                    var json = JsonConvert.SerializeObject(SelectedGame, _serializerSettings);
                    var gameFilePath = Path.Combine(_baseFolder, _dataFolder, SelectedGame.Id + ".json");
                    System.IO.File.Delete(gameFilePath);
                    System.IO.File.WriteAllText(gameFilePath, json);

                    var updateItem = new Item
                    {
                        ParentReference = new ItemReference
                        {
                            Id = _handledFolder.Id
                        }
                    };

                    await _client.Drive.Items[_items.ElementAt(_currentIndex).Id].Request().UpdateAsync(updateItem).ConfigureAwait(false);
                    await LoadDriveComponents().ConfigureAwait(false);
                    SelectedLevel = NextLevel();
                }).ConfigureAwait(false);

        });
        #endregion

        #region Properties
        public Visibility ProgressVisibility { get => _progressVisibility; set => SetAndRaise(ref _progressVisibility, value); }
        public Visibility SignButtonVisibility { get => _signButtonVisibility; set => SetAndRaise(ref _signButtonVisibility, value); }
        public ImageSource ImageOriginal { get => _imageOriginal; set => SetAndRaise(ref _imageOriginal, value); }
        public Visibility FormVisibility { get => _formVisibility; set => SetAndRaise(ref _formVisibility, value); }
        public ImageSource ImageCropped { get => _imageCropped; set => SetAndRaise(ref _imageCropped, value); }
        public IEnumerable<Game> Games { get => _games; set => SetAndRaise(ref _games, value); }
        public Game SelectedGame { get => _selectedGame; set => SetAndRaise(ref _selectedGame, value); }
        public Pack SelectedPack { get => _selectedPack; set => SetAndRaise(ref _selectedPack, value); }
        public Section SelectedSection { get => _selectedSection; set => SetAndRaise(ref _selectedSection, value); }
        public Level SelectedLevel { get => _selectedLevel; set => SetAndRaise(ref _selectedLevel, value); }
        public bool IsNextEnabled { get => _isNextEnabled; set => SetAndRaise(ref _isNextEnabled, value); }
        public bool IsPreviousEnabled { get => _isPreviousEnabled; set => SetAndRaise(ref _isPreviousEnabled, value); }
        public IEnumerable<Crop> Crops { get => _crops; set => _crops = value; }
        public Crop SelectedCrop
        {
            get => _selectedCrop;
            set
            {
                SetAndRaise(ref _selectedCrop, value);
                DoWithProgress(async () => await ShowCropped().ConfigureAwait(false)).ConfigureAwait(false);
            }
        }
        #endregion

        #region Private Methods
        private Item SelectRootFolder()
        {
            Item result = null;
            var resetEvent = new ManualResetEvent(false);
            DispatcherInvoke(() =>
            {
                var folderWindow = new FolderSelector(_client);
                folderWindow.ItemSelected += item =>
                {
                    result = item;
                    folderWindow.Close();
                    resetEvent.Set();
                };

                folderWindow.ShowDialog();
            });
            resetEvent.WaitOne();

            return result;
        }

        private string SelectFolder(string title)
        {
            var resetEvent = new ManualResetEvent(false);

            var result = string.Empty;

            DispatcherInvoke(() =>
            {
                var dialog = new FolderBrowserDialog
                {
                    Description = title,
                    ShowNewFolderButton = true,
                    SelectedPath = @"c:\Git\Bigsby\Games\docs"
                };

                dialog.ShowDialog();
                result = dialog.SelectedPath;
                resetEvent.Set();
            });

            resetEvent.WaitOne();
            return result;
        }

        private async Task Authenticate()
        {
            var msaAuthenticationProvider = new MsaAuthenticationProvider(_clientId, "https://login.live.com/oauth20_desktop.srf", new[] { "onedrive.readwrite" });
            await msaAuthenticationProvider.AuthenticateUserAsync().ConfigureAwait(false);
            _client = new OneDriveClient("https://api.onedrive.com/v1.0", msaAuthenticationProvider);
        }

        private async Task LoadDriveComponents()
        {
            _currentIndex = 0;

            _handledFolder = await _client.Drive.Root.ItemWithPath(_handledFolderPath).Request().GetAsync().ConfigureAwait(false);
            _othersFolder = await _client.Drive.Root.ItemWithPath(_otherFolderPath).Request().GetAsync().ConfigureAwait(false);

            var screenShotsChildren = await _client.Drive.Items[_rootFolder.Id].Children.Request().GetAsync().ConfigureAwait(false);

            _items = screenShotsChildren.Where(i => i.Image != null).ToList();

            IsPreviousEnabled = false;
            IsNextEnabled = _items.Count() > 0;

            await ShowImage().ConfigureAwait(false);
        }

        private async Task ShowImage()
        {
            if (_items.Count() == 0) return;

            ImageOriginal = DispatcherInvoke(() => new BitmapImage(new Uri(_items.ElementAt(_currentIndex).AdditionalData["@content.downloadUrl"] as string)));

            await ShowCropped().ConfigureAwait(false);

            // https://github.com/OneDrive/onedrive-sdk-csharp/blob/master/docs/items.md#moving-and-updating-an-item
        }

        private async Task ShowCropped()
        {
            using (var stream = await _client.Drive.Items[_items.ElementAt(_currentIndex).Id].Content.Request().GetAsync().ConfigureAwait(false))
            {
                var cropRect = new Rectangle(0, SelectedCrop.Start, 720, 720);
                var source = System.Drawing.Image.FromStream(stream);

                _croppedImage = new Bitmap(cropRect.Width, cropRect.Height);

                using (var g = Graphics.FromImage(_croppedImage))
                    g.DrawImage(source, new Rectangle(0, 0, _croppedImage.Width, _croppedImage.Height), cropRect, GraphicsUnit.Pixel);

                ImageCropped = DispatcherInvoke(() =>
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        _croppedImage.Save(memoryStream, ImageFormat.Png);
                        var result = new BitmapImage();
                        result.BeginInit();
                        result.StreamSource = memoryStream;
                        result.CacheOption = BitmapCacheOption.OnLoad;
                        result.EndInit();
                        return result;
                    }
                });
            }
        }

        private void LoadData()
        {
            var gamesJson = System.IO.File.ReadAllText(Path.Combine(_baseFolder, _dataFolder, "games.json"));

            var gamesList = JsonConvert.DeserializeObject<IEnumerable<Game>>(gamesJson);

            var result = new List<Game>();

            foreach (var game in gamesList)
            {
                var gameJson = System.IO.File.ReadAllText(Path.Combine(_baseFolder, _dataFolder, game.Id + ".json"));
                result.Add(JsonConvert.DeserializeObject<Game>(gameJson));
            }

            Games = result;
        }

        private Level NextLevel()
        {
            var next = false;
            foreach (var level in SelectedSection.Levels)
            {
                if (next) return level;
                next = level == SelectedLevel;
            }
            return null;
        }
        private async Task DoWithProgress(Func<Task> action)
        {
            ProgressVisibility = Visibility.Visible;
            await action().ConfigureAwait(false);
            ProgressVisibility = Visibility.Collapsed;
        }
        #endregion
    }
}
