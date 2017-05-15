using Microsoft.OneDrive.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml.Controls;

namespace UniversalFlowProcessor.ViewModels
{
    class FolderViewModel : FrameObservableObject
    {
        private readonly OneDriveClient _client;
        private IEnumerable<Item> _folders;
        private bool _isBackEnabled = false;
        private Item _current;
        private string _path;

        public FolderViewModel(Frame frame) : base(frame)
        {
            _client = Engine.Client;
        }

        public IEnumerable<Item> Folders { get => _folders; set => SetAndRaise(ref _folders, value); }
        public bool IsBackEnabled { get => _isBackEnabled; set => SetAndRaise(ref _isBackEnabled, value); }
        public Item Current { get => _current; set => SetAndRaise(ref _current, value); }
        public string Path { get => _path; set => SetAndRaise(ref _path, value); }

        public async Task Load()
        {
            await DoWithProgress(async () =>
            {
                Current = await _client.Drive.Root.Request().GetAsync().ConfigureAwait(false);
                SelectItem.Execute(Current);
            }).ConfigureAwait(false);
        }

        public ICommand SelectItem => new ActionCommand("", async p =>
        {
            await DoWithProgress(async () =>
            {
                var newItem = p as Item;
                if (null == newItem)
                    return;

                Current = await _client.Drive.Items[newItem.Id].Request().GetAsync().ConfigureAwait(false);
                var items = await _client.Drive.Items[Current.Id].Children.Request().GetAsync().ConfigureAwait(false);
                Folders = items.Where(i => null != i.Folder);
                IsBackEnabled = null != Current.ParentReference;
                Path = null == Current.ParentReference ?
                    "/" :
                    Current.ParentReference.Path.Remove(0, "/drive/root:".Length) + "/" + Current.Name;
            }).ConfigureAwait(false);
        });

        public ICommand Back => new ActionCommand("", async p =>
        {
            await DoWithProgress(async () =>
            {
                if (null == Current.ParentReference) return;
                IsBackEnabled = false;

                Current = await _client.Drive.Items[Current.ParentReference.Id].Request().GetAsync().ConfigureAwait(false);
                SelectItem.Execute(Current);
            }).ConfigureAwait(false);
        });

        public ICommand Select => new ActionCommand("", p =>
         {
             Engine.Source = Current;
             DoWithProgress(async () =>
             {
                 var folderPicker = new Windows.Storage.Pickers.FolderPicker();
                 folderPicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.Desktop;
                 folderPicker.FileTypeFilter.Add("*");
                 var folder = await folderPicker.PickSingleFolderAsync();
                 if (null == folder)
                     return;
                 Engine.Target = folder;

             }).ConfigureAwait(false);
         });//ItemSelected?.Invoke(Current));

        public event ItemSelectedEvent ItemSelected;
        public delegate void ItemSelectedEvent(Item item);
    }
}
