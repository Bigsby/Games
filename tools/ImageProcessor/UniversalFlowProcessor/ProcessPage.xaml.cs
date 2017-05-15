using System;
using UniversalFlowProcessor.ViewModels;
using Windows.UI.Core;
using Windows.UI.Popups;
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
            _viewModel.Error += async message => await new MessageDialog(message).ShowAsync();
            Loaded += async (s, e) => await _viewModel.Load().ConfigureAwait(false);
            KeyDown += (s, e) => 
            {
                var ctrl = Window.Current.CoreWindow.GetKeyState(Windows.System.VirtualKey.Control);
                if (!ctrl.HasFlag(CoreVirtualKeyStates.Down))
                    return;

                switch (e.Key)
                {
                    case Windows.System.VirtualKey.N:
                        _viewModel.NextImage.Execute(null);
                        break;
                    case Windows.System.VirtualKey.P:
                        _viewModel.PreviousImage.Execute(null);
                        break;
                    case Windows.System.VirtualKey.S:
                        _viewModel.SaveSolution.Execute(null);
                        break;
                    case Windows.System.VirtualKey.I:
                        _viewModel.SaveInitial.Execute(null);
                        break;
                    case Windows.System.VirtualKey.O:
                        _viewModel.MoveToOther.Execute(null);
                        break;
                    case Windows.System.VirtualKey.D:
                        _viewModel.SaveData.Execute(null);
                        break;
                }
            };
        }
    }
}
