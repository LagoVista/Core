using LagoVista.Core.Interfaces;
using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace LagoVista.Core.ServiceCommon
{
    public abstract class ServiceBase : IServiceBase, INotifyPropertyChanged
    {
        public ServiceBase()
        {
            ConnectionSettings = new LagoVista.Core.Models.ConnectionSettings();
            ConnectionSettings.ValidationAction = () => true;
            ConnectionSettings.GetValidationErrors = () => "NOT CONFIGURED";
            PropertiesUpdatedOnMainThread = false;
        }

        public event EventHandler Connected;
        public event EventHandler Disconnected;

        public virtual async Task InitAsync()
        {
            ConnectionSettings.UserName = await PlatformSupport.Services.Storage.GetKVPAsync<string>(String.Format("{0}_USER_ID", GetCredentialsPrefix()));
            ConnectionSettings.DeviceId = await PlatformSupport.Services.Storage.GetKVPAsync<string>(String.Format("{0}_DEVICE_ID", GetCredentialsPrefix()));
            ConnectionSettings.Password = await PlatformSupport.Services.Storage.GetKVPAsync<string>(String.Format("{0}_PASSWORD", GetCredentialsPrefix()));
            ConnectionSettings.Uri = await PlatformSupport.Services.Storage.GetKVPAsync<string>(String.Format("{0}_SERVICE_ADDRESS", GetCredentialsPrefix()));
            ConnectionSettings.Port = await PlatformSupport.Services.Storage.GetKVPAsync<string>(String.Format("{0}_SERVICE_PORT", GetCredentialsPrefix()));
            ConnectionSettings.IsSSL = Convert.ToBoolean(await PlatformSupport.Services.Storage.GetKVPAsync<string>(String.Format("{0}_IS_SSL", GetCredentialsPrefix()), false.ToString()));
        }

        public async Task SaveCredentialsAsync()
        {
            await PlatformSupport.Services.Storage.StoreKVP(String.Format("{0}_USER_ID", GetCredentialsPrefix()), ConnectionSettings.UserName);
            await PlatformSupport.Services.Storage.StoreKVP(String.Format("{0}_DEVICE_ID", GetCredentialsPrefix()), ConnectionSettings.DeviceId);
            await PlatformSupport.Services.Storage.StoreKVP(String.Format("{0}_PASSWORD", GetCredentialsPrefix()), ConnectionSettings.Password);
            await PlatformSupport.Services.Storage.StoreKVP(String.Format("{0}_SERVICE_ADDRESS", GetCredentialsPrefix()), ConnectionSettings.Uri);
            await PlatformSupport.Services.Storage.StoreKVP(String.Format("{0}_SERVICE_PORT", GetCredentialsPrefix()), ConnectionSettings.Port);
            await PlatformSupport.Services.Storage.StoreKVP(String.Format("{0}_IS_SSL", GetCredentialsPrefix()), ConnectionSettings.IsSSL.ToString());
        }

        public event PropertyChangedEventHandler PropertyChanged;
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

        protected bool Set<T>(ref T storage, T value, string columnName = null, [CallerMemberName] string propertyName = null)
        {
            if (object.Equals(storage, value)) return false;

            storage = value;
            if (PropertyChanged != null && !String.IsNullOrEmpty(propertyName))
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));

            return true;
        }

        public void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null && !String.IsNullOrEmpty(propertyName))
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }        

        IConnectionSettings _connectionSettings;
        public IConnectionSettings ConnectionSettings
        {
            get { return _connectionSettings; }
            set { Set(ref _connectionSettings, value); }
        }

        public bool HasAuthInfo
        {
            get { return ConnectionSettings.ValidationAction(); }
        }

        private bool _isConnected;
        public virtual bool IsConnected
        {
            get { return _isConnected; }
            set
            {
                if (_isConnected != value)
                {
                    _isConnected = value;
                    RaisePropertyChanged();
                    if (value && Connected != null)
                        Connected(this, null);

                    if (!value && Disconnected != null)
                        Disconnected(this, null);
                }
            }
        }


        private bool _showDiagnostics = false;
        public bool ShowDiagnostics
        {
            get { return _showDiagnostics; }
            set { _showDiagnostics = value; }
        }


        public virtual string GetCredentialsPrefix()
        {
            return GetType().ToString();
        }

        protected void Log(String message, params object[] args)
        {
            if (ShowDiagnostics)
            {
                if (args != null && args.Length > 0)
                    PlatformSupport.Services.Logger.AddCustomEvent(PlatformSupport.LogLevel.Message, GetType().Name, String.Format(message, args));
                else
                    PlatformSupport.Services.Logger.AddCustomEvent(PlatformSupport.LogLevel.Message, GetType().Name, message);
            }
        }

        protected void LogDetails(String message, params object[] args)
        {
            if (ShowDiagnostics)
            {
                if (args != null && args.Length > 0)
                    PlatformSupport.Services.Logger.AddCustomEvent(PlatformSupport.LogLevel.Verbose, GetType().Name, String.Format(message, args));
                else
                    PlatformSupport.Services.Logger.AddCustomEvent(PlatformSupport.LogLevel.Verbose, GetType().Name, message);
            }
        }

        protected void LogException(String area, Exception ex)
        {
            PlatformSupport.Services.Logger.AddException(GetType().Name, ex);
        }

        private DateTime? _dateConnected;
        public DateTime? DateConnected
        {
            get { return _dateConnected; }
            protected set { Set(ref _dateConnected, value); }
        }

        private DateTime? _dateLastMessage;
        public DateTime? DateLastMessage
        {
            get { return _dateLastMessage; }
            protected set { Set(ref _dateLastMessage, value); }
        }

        private DateTime? _dateDisconnected;
        public DateTime? DateDisconnected
        {
            get { return _dateDisconnected; }
            protected set { Set(ref _dateDisconnected, value); }
        }

        public bool PropertiesUpdatedOnMainThread { get; set; }
    }
}
