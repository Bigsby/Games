using Microsoft.OneDrive.Sdk;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using System.Runtime.InteropServices.WindowsRuntime;

namespace UniversalFlowProcessor.ViewModels
{
    class ProcessViewModel : FrameObservableObject
    {
        private IEnumerable<Item> _items;
        private int _currentIndex;
        private bool _isPreviousEnabled;
        private bool _isNextEnabled;
        private IEnumerable<Game> _games;
        private Game _selectedGame;
        private Pack _selectedPack;
        private Section _selectedSection;
        private Level _selectedLevel;
        private IEnumerable<Color> _colors;
        private ImageSource _solution;
        private ImageSource _header;
        private IEnumerable<Crop> _crops;
        private Crop _selectedCrop;
        private Stream _currentImageStream;

        public ProcessViewModel(Frame frame) : base(frame)
        {
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
            SelectedCrop = _crops.ElementAt(0);
        }

        public bool IsPreviousEnabled { get => _isPreviousEnabled; set => SetAndRaise(ref _isPreviousEnabled, value); }
        public bool IsNextEnabled { get => _isNextEnabled; set => SetAndRaise(ref _isNextEnabled, value); }
        public IEnumerable<Game> Games { get => _games; set => SetAndRaise(ref _games, value); }
        public IEnumerable<Color> Colors { get => _colors; set => SetAndRaise(ref _colors, value); }
        public Game SelectedGame { get => _selectedGame; set => SetAndRaise(ref _selectedGame, value); }
        public Pack SelectedPack { get => _selectedPack; set => SetAndRaise(ref _selectedPack, value); }
        public Section SelectedSection { get => _selectedSection; set => SetAndRaise(ref _selectedSection, value); }
        public Level SelectedLevel { get => _selectedLevel; set => SetAndRaise(ref _selectedLevel, value); }
        public ImageSource Solution { get => _solution; set => SetAndRaise(ref _solution, value); }
        public IEnumerable<Crop> Crops { get => _crops; set => SetAndRaise(ref _crops, value); }
        public Crop SelectedCrop { get => _selectedCrop; set => SetAndRaise(ref _selectedCrop, value); }
        public ImageSource Header { get => _header; set => SetAndRaise(ref _header, value); }

        public async Task Load()
        {
            await DoWithProgress(async () =>
            {
                _currentIndex = 0;
                var children = await Engine.Client.Drive.Items[Engine.Source.Id].Children.Request().GetAsync().ConfigureAwait(false);
                _items = children.Where(i => i.Image != null).ToList();
                await DispatcherInvoke(() => Windows.UI.Xaml.Window.Current.SetTitleBar(new TextBlock { Text = $"Flow Image Processor ({Engine.Source.Folder.ChildCount})" })).ConfigureAwait(false);

                IsPreviousEnabled = false;
                IsNextEnabled = _items.Count() > 0;
                await LoadData().ConfigureAwait(false);
                await ProcessImages().ConfigureAwait(false);
            }).ConfigureAwait(false);
        }

        private async Task LoadData()
        {
            var dataFolder = await Engine.DataFolder().ConfigureAwait(false);
            var gamesFile = await dataFolder.GetFileAsync("games.json");
            var gamesJson = await FileIO.ReadTextAsync(gamesFile);

            var gamesList = JsonConvert.DeserializeObject<IEnumerable<Game>>(gamesJson);

            var result = new List<Game>();

            foreach (var game in gamesList)
            {
                if (null != game.Colors)
                    Colors = game.Colors.Select(color => (Color)XamlBindingHelper.ConvertValue(typeof(Color), "#" + color));

                var gameFile = await dataFolder.GetFileAsync(game.Id + ".json");
                var gameJson = await FileIO.ReadTextAsync(gameFile);
                result.Add(JsonConvert.DeserializeObject<Game>(gameJson));
            }

            Games = result;
        }

        private async Task ProcessImages()
        {
            if (_items.Count() == 0) return;

            using (var stream = await Engine.Client.Drive.Items[_items.ElementAt(_currentIndex).Id].Content.Request().GetAsync().ConfigureAwait(false))
            {
                Solution = await (await DispatcherInvoke(() => CropImage(stream, 0, SelectedCrop.Start, Engine.ImageSize, Engine.ImageSize)).ConfigureAwait(false)).ConfigureAwait(false);
                Header = await (await DispatcherInvoke(() => CropImage(stream, 0, 0, Engine.ImageSize, (int)SelectedCrop.Start)).ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        private async Task<ImageSource> CropImage(Stream stream, uint startX, uint startY, int width, int height)
        {
            var decoder = await BitmapDecoder.CreateAsync(stream.AsRandomAccessStream());
            var transform = new BitmapTransform
            {
                Bounds = new BitmapBounds
                {
                    X = startX,
                    Y = startY,
                    Height = (uint)height,
                    Width = (uint)width,
                }
            };

            var pix = await decoder.GetPixelDataAsync(
                BitmapPixelFormat.Bgra8,
                BitmapAlphaMode.Straight,
                transform,
                ExifOrientationMode.IgnoreExifOrientation,
                ColorManagementMode.ColorManageToSRgb);

            var pixels = pix.DetachPixelData();


            var cropBmp = new WriteableBitmap(width, height);
            var pixStream = cropBmp.PixelBuffer.AsStream();
            pixStream.Write(pixels, 0, width * height * 4);
            return cropBmp;
        }
    }
}