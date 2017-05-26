#region License
/*Copyright (c) 2017, Software Logistics, LLC
All rights reserved.

Redistribution and use in source and binary forms, with or without modification, are permitted provided that 
the following conditions are met:

* Redistributions of source code must retain the above copyright notice, this list of conditions and the following 
disclaimer.

* Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the 
following disclaimer in the documentation and/or other materials provided with the distribution.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS
OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF 
MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE 
COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, 
EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF 
SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER 
CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING 
NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED 
OF THE POSSIBILITY OF SUCH DAMAGE.*/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using LagoVista.Core.PlatformSupport;
using LagoVista.Core.Commanding;
using LagoVista.Core.Models;
using LagoVista.Core.Resources;

namespace LagoVista.Core.ViewModels
{
    public abstract class ViewModelBase : INotifyPropertyChanged, IDataErrorInfo
    {
        #region INotifyPropertyChanged Member

        RelayCommand _closeScreenCommand;
        public event PropertyChangedEventHandler PropertyChanged;

        public IViewModelNavigation ViewModelNavigation { get { return IOC.SLWIOC.Get<IViewModelNavigation>(); } }

        protected void OnPropertyChanged<TResult>(Expression<Func<TResult>> propertyExpression)
        {
            if (!this.CheckExpressionForMemberAccess(propertyExpression.Body))
                throw new ArgumentException("propertyExpression",
                        string.Format("The expected expression is no 'MemberAccess'; its a '{0}'", propertyExpression.Body.NodeType));


            if (propertyExpression == null)
                throw new ArgumentNullException("propertyExpression");

            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(this.GetPropertyNameFromExpression(propertyExpression)));
        }

        private bool CheckExpressionForMemberAccess(System.Linq.Expressions.Expression propertyExpression)
        {
            return propertyExpression.NodeType == ExpressionType.MemberAccess;
        }

        public string GetPropertyNameFromExpression<TResult>(Expression<Func<TResult>> propertyExpression)
        {
            System.Linq.Expressions.MemberExpression memberExpression = (System.Linq.Expressions.MemberExpression)propertyExpression.Body;

            if (memberExpression != null)
            {
                return memberExpression.Member.Name;
            }
            else
                throw new ArgumentException("propertyExpression");
        }

        protected bool Set<T>(ref T storage, T value, string columnName = null, [CallerMemberName] string propertyName = null)
        {
            if (object.Equals(storage, value)) return false;

            storage = value;
            this.RaisePropertyChanged(propertyName);
            return true;
        }

        protected void RaisePropertyChanged<T>(Expression<Func<T>> property)
        {
            var eventHandler = this.PropertyChanged;
            if (eventHandler != null)
            {
                var expr = property.Body as MemberExpression;
                if (expr != null)
                    eventHandler(this, new PropertyChangedEventArgs(expr.Member.Name));
            }
        }


        protected void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            var eventHandler = this.PropertyChanged;
            if (eventHandler != null)
            {
                eventHandler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        #region Attributes

        private Boolean? mbIsDialogClose;

        #endregion

        #region Constructor / Destructor

        public ViewModelBase()
        {
            this.ErrorList = new Dictionary<string, string>();
            _closeScreenCommand = new RelayCommand(() => CloseScreen());


        }

        #endregion

        #region Public Methods / Properties

        public Dictionary<string, string> ErrorList { get; private set; }

        public Boolean? IsDialogClose
        {
            get { return mbIsDialogClose; }
            set
            {
                mbIsDialogClose = value;
                this.OnPropertyChanged(() => this.IsDialogClose);
            }
        }

        public void SetParameter(Object parameter)
        {
            _navigationParameter = parameter;
        }

        public Object NavigationParameter { get { return _navigationParameter; } }

        private Object _navigationParameter;

        public virtual Task InitAsync()
        {
            return Task.FromResult(0);
        }

        public virtual Task ReloadedAsync()
        {
            return Task.FromResult(0);
        }
        
        public virtual Task IsClosingAsync()
        {
            return Task.FromResult(0);
        }

        public virtual string Error
        {
            get { return string.Empty; }
        }

        public virtual string this[string columnName]
        {
            get
            {
                if (this.ErrorList.ContainsKey(columnName))
                    return this.ErrorList[columnName];
                return string.Empty;
            }
        }

        public IDispatcherServices DispatcherServices
        {
            get { return IOC.SLWIOC.Get<IDispatcherServices>(); }
        }

        public ViewModelLaunchArgs LaunchArgs
        {
            get; set;
        }

        public bool SectionVisibility
        {
            get;
            set;
        }

        private string _messageText;
        public String MessageText
        {
            get { return _messageText; }
            set { Set(ref _messageText, value); }
        }

        private int _isBusyCount;
        public bool IsBusy
        {
            set
            {
                lock (this)
                {
                    int prevBusyCount = _isBusyCount;
                    if (value)
                        _isBusyCount++;
                    else
                        _isBusyCount--;

                    if (_isBusyCount < 0)
                        _isBusyCount = 0;

                    if ((prevBusyCount == 0 && _isBusyCount > 0) || (prevBusyCount > 0 && _isBusyCount == 0))
                        RaisePropertyChanged();
                }
            }
            get { return _isBusyCount > 0; }
        }

        private String _busyMessage;
        public String BusyMessage
        {
            get { return _busyMessage; }
            set { Set(ref _busyMessage, value); }
        }

        public void LogTelemetry(String msg, [CallerMemberName] string area = "", params KeyValuePair<string, string>[] args)
        {
            area = $"{this.GetType().Name}_{area}";
            Logger.Log(LogLevel.Message, area, msg, args);
        }
       
        public virtual void TransitionCompleted()
        {

        }

        public IDeviceManager DeviceManager
        {
            get { return IOC.SLWIOC.Get<IDeviceManager>(); }
        }

        public IStorageService Storage
        {
            get { return IOC.SLWIOC.Get<IStorageService>(); }
        }

        public IPopupServices Popups
        {
            get { return IOC.SLWIOC.Get<IPopupServices>(); }
        }
        
        public ILogger Logger
        {
            get { return IOC.SLWIOC.Get<ILogger>(); }
        }

        public async Task CloseScreenAsync()
        {
            await ViewModelNavigation.GoBackAsync();
        }

        public async void CloseScreen()
        {
            await ViewModelNavigation.GoBackAsync();
        }


        public bool IsNetworkConnected
        {
            get { return PlatformSupport.Services.Network.IsInternetConnected; }
        }

        protected async Task<bool> PerformNetworkOperation(Func<Task> action, String waitMessage = null)
        {
            var success = false;
            if (!IsNetworkConnected)
                MessageText = LagoVistaCommonStrings.NoConnection;
            else
            {
                try
                {
                    BusyMessage = String.IsNullOrEmpty(waitMessage) ? LagoVistaCommonStrings.Common_PleaseWait : waitMessage;
                    IsBusy = true;
                    await action();
                    success = true;
                }
                catch (Exception ex)
                {
                    success = false;
                    PlatformSupport.Services.Logger.LogException("ViewModelBase.PerformNetworkOperation", ex);
                }
                finally
                {
                    IsBusy = false;
                }

                if (success == false)
                    MessageText = LagoVistaCommonStrings.ErrorMakingCall;
            }

            return success;
        }
        #endregion

        public RelayCommand CloseScreenCommand { get { return _closeScreenCommand; } }

    }

    public interface IDataErrorInfo
    {
        // Summary:
        //     Gets an error message indicating what is wrong with this object.
        //
        // Returns:
        //     An error message indicating what is wrong with this object. The default is
        //     an empty string ("").
        string Error { get; }

        // Summary:
        //     Gets the error message for the property with the given name.
        //
        // Parameters:
        //   columnName:
        //     The name of the property whose error message to get.
        //
        // Returns:
        //     The error message for the property. The default is an empty string ("").
        string this[string columnName] { get; }
    }
}
