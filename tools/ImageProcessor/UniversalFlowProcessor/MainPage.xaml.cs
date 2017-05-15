using UniversalFlowProcessor.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace UniversalFlowProcessor
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private readonly MainViewModel _viewModel;

        public MainPage()
        {
            this.InitializeComponent();
            DataContext = _viewModel = new MainViewModel(Window.Current.Content as Frame);
            Loaded += (s, e) => _viewModel.SignIn.Execute(null);
        }
    }
}
