using ImageProcessor.ViewModels;
using System.Windows;

namespace ImageProcessor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const string _clientId = "000000004018EE88";
        //const string dataFolder = @"C:\Git\Bigsby\Games\docs\data";

        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel(_clientId, Dispatcher);
        }
    }
}
