using UniversalFlowProcessor.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Linq;
// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace UniversalFlowProcessor
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FolderSelector : Page
    {
        private readonly FolderViewModel _viewModel;
        public FolderSelector()
        {
            this.InitializeComponent();
            DataContext = _viewModel = new FolderViewModel(Window.Current.Content as Frame);
            Loaded += async (s, e) => await _viewModel.Load();
            FolderList.SelectionChanged += (s, e) => _viewModel.SelectItem.Execute(e.AddedItems.FirstOrDefault());
        }
    }
}
