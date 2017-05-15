using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;

namespace UniversalFlowProcessor.ViewModels
{
    public abstract class FrameObservableObject : ObservableObject
    {
        protected readonly Frame Frame;
        private bool _working;
        private bool _notWorking;

        public FrameObservableObject(Frame frame)
        {
            Frame = frame;
            IsWorking = false;
            NotWorking = true;
        }

        protected override async void RaisePropertyChangedInternal(string propertyName)
        {
            await Frame.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => base.RaisePropertyChangedInternal(propertyName));
        }

        protected async Task DispatcherInvoke(DispatchedHandler action)
        {
            await Frame.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, action);
        }

        protected async Task<T> DispatcherInvoke<T>(Func<T> func)
        {
            var result = default(T);
            await Frame.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => result = func());
            return result;
        }
        public bool IsWorking { get => _working; set => SetAndRaise(ref _working, value); }
        public bool NotWorking { get => _notWorking; set => SetAndRaise(ref _notWorking, value); }

        protected async Task DoWithProgress(Func<Task> action)
        {
            IsWorking = true;
            NotWorking = false;
            await action().ConfigureAwait(false);
            IsWorking = false;
            NotWorking = true;
        }
    }

    public abstract class ObservableObject : INotifyPropertyChanged
    {
        #region Protected Methods
        protected void RaiseEvent(EventHandler handler)
        {
            handler?.Invoke(this, EventArgs.Empty);
        }

        protected void RaiseEvent<T>(EventHandler<T> handler, Func<T> argsFunc) where T : EventArgs
        {
            handler?.Invoke(this, argsFunc());
        }

        protected void SetAndRaise<T>(ref T field, T newValue, [CallerMemberName]string propertyName = "")
        {
            if (string.IsNullOrEmpty(propertyName)) return;

            if (Equals(field, newValue))
                return;

            field = newValue;
            RaisePropertyChangedInternal(propertyName);
        }

        protected void RaisePropertyChanged([CallerMemberName]string propertyName = "")
        {
            RaisePropertyChangedInternal(propertyName);
        }

        #endregion

        #region Property Changed
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChangedInternal(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }

    public class ActionCommand : ObservableObject, ICommand
    {
        private readonly Func<object, bool> _canExecute;
        private readonly Action<object> _action;
        private bool _canExecuteState;
        private string _caption;
        private bool _isVisible;

        #region Constructors
        public ActionCommand(string caption, Action<object> action)
        {
            Caption = caption;
            _action = action;
        }

        public ActionCommand(string caption, Action<object> action, Func<object, bool> canExecute)
            : this(caption, action)
        {
            _canExecute = canExecute;
        }
        #endregion

        #region Properties

        public string Caption
        {
            get { return _caption; }
            set { SetAndRaise(ref _caption, value); }
        }

        public bool IsVisible
        {
            get { return _isVisible; }
            set { SetAndRaise(ref _isVisible, value); }
        }

        #endregion

        #region ICommand Members
        public bool CanExecute(object parameter)
        {
            var newState = (null != _canExecute && _canExecute(parameter))
                ||
                null != _action;

            if (newState != _canExecuteState)
            {
                _canExecuteState = newState;
                RaiseEvent(CanExecuteChanged);
            }

            return _canExecuteState;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            if (CanExecute(parameter))
                _action(parameter);
        }
        #endregion
    }
}
