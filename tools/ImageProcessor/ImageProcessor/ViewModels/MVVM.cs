using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Threading;

namespace ImageProcessor.ViewModels
{
    public abstract class DispatcherObservableObject : ObservableObject
    {
        protected readonly Dispatcher Dispatcher;

        public DispatcherObservableObject(Dispatcher dispatcher)
        {
            Dispatcher = dispatcher;
        }

        protected override void RaisePropertyChangedInternal(string propertyName)
        {
            Dispatcher.Invoke(() => base.RaisePropertyChangedInternal(propertyName));
        }

        protected void DispatcherInvoke(Action action)
        {
            Dispatcher.Invoke(action);
        }

        protected T DispatcherInvoke<T>(Func<T> func)
        {
            return Dispatcher.Invoke(func);
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
