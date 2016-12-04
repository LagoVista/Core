using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LagoVista.Core.Commanding
{
    public class RelayCommand : ICommand
    {
        #region Events

        public event EventHandler CanExecuteChanged;

        #endregion

        #region Fields

        readonly Action _noParamsExecute;
        readonly Action<object> _execute;
        readonly Predicate<object> _canExecute;


        #endregion // Fields

        #region Constructors

        public RelayCommand(Action execute)        
        {
            _noParamsExecute = execute;
        }

        public RelayCommand(Action<object> execute)
            : this(execute, null)
        {
        }

        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            _execute = execute;
            this._canExecute = canExecute;
        }

        #endregion

        #region ICommand Members

        public bool CanExecute(object parameter)
        {
            return _canExecute == null ? true : _canExecute(parameter);
        }

        public void RaiseCanExecuteChanged()
        {
            if (CanExecuteChanged != null) CanExecuteChanged(this, EventArgs.Empty);
        }

        public void Execute(object parameter)
        {
            if (_noParamsExecute != null)
                _noParamsExecute();

            if(_execute != null)
                _execute(parameter);
        }

        #endregion
    }
}
