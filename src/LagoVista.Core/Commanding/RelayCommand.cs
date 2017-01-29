using System;
using System.Windows.Input;

namespace LagoVista.Core.Commanding
{
    public class RelayCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        readonly Action _execute;
        readonly Action<object> _executeParam;
        readonly Func<object, bool> _canExecuteParam;
        readonly Func<bool> _canExecute;

        public RelayCommand(Action execute)        
        {
            _execute = execute;
        }

        public RelayCommand(Action<object> execute)
        {
            _executeParam = execute;
        }

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute)
        {
            _executeParam = execute;
            _canExecuteParam = canExecute;
        }

        public RelayCommand(Action execute, Func<bool> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public virtual bool CanExecute(object parameter)
        {
            if(_canExecute != null)
            {
                return _canExecute();
            }

            if(_canExecuteParam != null)
            {
                return _canExecuteParam(parameter);
            }

            return true;
        }

        public void RaiseCanExecuteChanged()
        {
            if (CanExecuteChanged != null) CanExecuteChanged(this, EventArgs.Empty);
        }

        public void Execute(object parameter)
        {
            if (_executeParam != null && (_canExecuteParam == null || _canExecuteParam(parameter)))
                _executeParam(parameter);

            if(_execute != null && (_canExecute == null || _canExecute()))
                _execute();
        }
    }
}
