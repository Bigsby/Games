using ImageProcessor.ViewModels;
using Microsoft.OneDrive.Sdk;
using System.Windows;

namespace ImageProcessor
{
    /// <summary>
    /// Interaction logic for FolderSelector.xaml
    /// </summary>
    public partial class FolderSelector : Window
    {
        private readonly FolderViewModel _viewModel;
        public FolderSelector(OneDriveClient client)
        {
            InitializeComponent();
            DataContext = _viewModel = new FolderViewModel(client, Dispatcher);
            _viewModel.ItemSelected += item => ItemSelected?.Invoke(item);
            Loaded += async (s, e) => await _viewModel.Load().ConfigureAwait(false);
        }

        public event ItemSelectedEvent ItemSelected;
        public delegate void ItemSelectedEvent(Item item);
    }
}
