using ImageProcessor.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace ImageProcessor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const string _clientId = "000000004018EE88";
        private readonly MainViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = _viewModel = new MainViewModel(_clientId, Dispatcher);
            KeyDown += (s, e) =>
            {
                if (!(Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
                    return;

                e.Handled = true;
                if (e.IsRepeat)
                    return;

                switch (e.Key)
                {
                    case Key.S:
                        _viewModel.Save.Execute(null);
                        break;
                    case Key.I:
                        _viewModel.SaveInitial.Execute(null);
                        break;
                    case Key.P:
                        _viewModel.SelectPreviousLevel();
                        break;
                    case Key.N:
                        _viewModel.SelectNextLevel();
                        break;
                    case Key.Right:
                        if (_viewModel.IsNextEnabled)
                            _viewModel.Next.Execute(null);
                        break;
                    case Key.Left:
                        if (_viewModel.IsPreviousEnabled)
                            _viewModel.Previous.Execute(null);
                        break;
                    case Key.O:
                        _viewModel.MoveToOther.Execute(null);
                        break;
                }

            };
        }
    }
}
