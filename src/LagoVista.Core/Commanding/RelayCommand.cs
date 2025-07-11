﻿using LagoVista.Core.IOC;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LagoVista.Core.Commanding
{
    public class RelayCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        object _parameter;

        readonly Action _execute;
        readonly Action<object> _executeParam;
        readonly Func<object, bool> _canExecuteParam;
        readonly Func<bool> _canExecute;
        readonly Timer _timer;

        public RelayCommand(Action execute)
        {
            _execute = execute;
            _timer = new Timer((state) => DebounceTimedout(), null, Timeout.Infinite, Timeout.Infinite);
        }

        public RelayCommand(Action<object> execute)
        {
            _executeParam = execute;
            _timer = new Timer((state) => DebounceTimedout(), null, Timeout.Infinite, Timeout.Infinite);
        }

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute)
        {
            _executeParam = execute;
            _canExecuteParam = canExecute;
            _timer = new Timer((state) => DebounceTimedout(), null, Timeout.Infinite, Timeout.Infinite);
        }

        public RelayCommand(Action execute, Func<bool> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
            _timer = new Timer((state) => DebounceTimedout(), null, Timeout.Infinite, Timeout.Infinite);
        }

        public virtual bool CanExecute(object parameter)
        {
            if (_debounceHigh)
                return false;

            if (_canExecute != null)
            {
                return _canExecute();
            }

            if (_canExecuteParam != null)
            {
                return _canExecuteParam(parameter);
            }

            return Enabled;
        }

        public bool _debounceHigh = false;

        private void DebounceTimedout()
        {
            _debounceHigh = false;
        }

        public TimeSpan DebouncePeriod { get; set; } = TimeSpan.FromMilliseconds(250);

        private bool _enabled = true;
        public bool Enabled
        {
            get { return _enabled; }
            set
            {
                if (_enabled != value)
                {
                    _enabled = value;
                    RaiseCanExecuteChanged();
                }
            }
        }

        public void RaiseCanExecuteChanged()
        {
            if (CanExecuteChanged != null) CanExecuteChanged(this, EventArgs.Empty);
        }



        public void Execute(object parameter)
        {
            if (DebounceEnabled)
            {
                RaiseCanExecuteChanged();
                _timer.Change((int)DebouncePeriod.TotalMilliseconds, Timeout.Infinite);
            }

            if (_executeParam != null && (_canExecuteParam == null || _canExecuteParam(parameter == null ? _parameter : parameter)))
            {
                _executeParam(parameter == null ? _parameter : parameter);
            }
            else if (_execute != null && (_canExecute == null || _canExecute()))
            {
                _execute();
            }
        }

        public bool DebounceEnabled { get; set; } = true;

        public static RelayCommand Create(Action<object> action)
        {
            return new RelayCommand(action);
        }

        public static RelayCommand Create(Action<Object> action, Object parameter)
        {
            return new RelayCommand(action) { _parameter = parameter };
        }

        public static RelayCommand Create(Action action)
        {
            return new RelayCommand(action);
        }
    }

    public class RelayCommand<TParam> : ICommand
    {
        TParam _parameter;
        Action<TParam> _cmdAction;
        Func<TParam, bool> _canExecuteParam;

        public event EventHandler CanExecuteChanged;

        public static RelayCommand<TParam> Create(Action<TParam> action)
        {
            return new RelayCommand<TParam>() { _cmdAction = action };
        }

        public static RelayCommand<TParam> Create(Action<TParam> action, TParam parameter)
        {
            return new RelayCommand<TParam>() { _parameter = parameter, _cmdAction = action };
        }

        public static RelayCommand<TParam> Create(Action<TParam> action, TParam parameter, Func<TParam, bool> canExecute)
        {
            return new RelayCommand<TParam>() { _parameter = parameter, _cmdAction = action, _canExecuteParam = canExecute };
        }

        public static RelayCommand<TParam> Create(Action<TParam> action, Func<TParam, bool> canExecute)
        {
            return new RelayCommand<TParam>() { _cmdAction = action, _canExecuteParam = canExecute };
        }

        public void RaiseCanExecuteChanged()
        {
            IDispatcherServices dispatcher;
            if (SLWIOC.TryResolve(out dispatcher))
            {
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private bool _enabled = true;
        public bool Enabled
        {
            get { return _enabled; }
            set
            {
                if (_enabled != value)
                {
                    _enabled = value;
                    RaiseCanExecuteChanged();
                }
            }
        }

        public bool CanExecute(object parameter = null)
        {
            if (!Enabled)
                return false;

            if (_canExecuteParam != null)
                return _canExecuteParam(parameter != null ? (TParam)parameter : _parameter);

            return true;
        }

        public void Execute(object parameter = null)
        {
            _cmdAction(parameter != null ? (TParam)parameter : _parameter);
        }
    }

}
