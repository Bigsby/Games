using Microsoft.OneDrive.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace ImageProcessor.ViewModels
{
    public class FolderViewModel : DispatcherObservableObject
    {
        private readonly OneDriveClient _client;
        private IEnumerable<Item> _folders;
        private Visibility _progressVisibility = Visibility.Collapsed;
        private bool _isBackEnabled = false;
        private Item _current;

        public FolderViewModel(OneDriveClient client, Dispatcher dispatcher) : base(dispatcher)
        {
            _client = client;
        }

        public IEnumerable<Item> Folders { get => _folders; set => SetAndRaise(ref _folders, value); }
        public Visibility ProgressVisibility { get => _progressVisibility; set => SetAndRaise(ref _progressVisibility, value); }
        public bool IsBackEnabled { get => _isBackEnabled; set => SetAndRaise(ref _isBackEnabled, value); }

        public async Task Load()
        {
            await DoWithProgress(async () =>
            {
                _current = await _client.Drive.Root.Request().GetAsync().ConfigureAwait(false);
                SelectItem.Execute(_current);
            }).ConfigureAwait(false);
        }

        public ICommand SelectItem => new ActionCommand("", async p =>
        {
            await DoWithProgress(async () =>
            {
                _current = p as Item;
                var items = await _client.Drive.Items[_current.Id].Children.Request().GetAsync().ConfigureAwait(false);
                Folders = items.Where(i => null != i.Folder);
                IsBackEnabled = null != _current.ParentReference;
            }).ConfigureAwait(false);
        });

        public ICommand Back => new ActionCommand("", async p =>
        {
            await DoWithProgress(async () => 
            {
                if (null == _current.ParentReference) return;

                _current = await _client.Drive.Items[_current.ParentReference.Id].Request().GetAsync().ConfigureAwait(false);
                SelectItem.Execute(_current);
            }).ConfigureAwait(false);
        });

        public ICommand Select => new ActionCommand("", p => ItemSelected?.Invoke(_current));

        

        private async Task DoWithProgress(Func<Task> action)
        {
            ProgressVisibility = Visibility.Visible;
            await action().ConfigureAwait(false);
            ProgressVisibility = Visibility.Collapsed;
        }

        public event ItemSelectedEvent ItemSelected;
        public delegate void ItemSelectedEvent(Item item);
    }
}
