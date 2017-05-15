using UniversalFlowProcessor.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace UniversalFlowProcessor
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ProcessPage : Page
    {
        private readonly ProcessViewModel _viewModel;
        public ProcessPage()
        {
            this.InitializeComponent();
            DataContext = _viewModel = new ProcessViewModel(Window.Current.Content as Frame);
            Loaded += async (s, e) => await _viewModel.Load().ConfigureAwait(false);
        }
    }
}
