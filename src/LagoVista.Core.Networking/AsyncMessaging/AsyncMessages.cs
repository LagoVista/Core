using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace LagoVista.Core.Networking.AsyncMessaging
{
    public class AsyncMessage : IAsyncMessage
    {
        private const string _privateKeyPrefix = "__";
        private const string _pathKey = _privateKeyPrefix + "message_path";
        private const string _idKey = _privateKeyPrefix + "message_id";
        private const string _correlationIdKey = _privateKeyPrefix + "correlation_id";
        private const string _dateTimeStampKey = _privateKeyPrefix + "datetimestamp_key";

        private static readonly JsonSerializerSettings _jsonSettings = new JsonSerializerSettings
        {
            // note: do not change this to auto or array or all - only works with TypeNameHandling.Objects
            TypeNameHandling = TypeNameHandling.Objects,
            Formatting = Newtonsoft.Json.Formatting.None,
            NullValueHandling = NullValueHandling.Ignore,
            MissingMemberHandling = MissingMemberHandling.Ignore,
            TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
        };

        private Dictionary<string, object> _data = new Dictionary<string, object>();

        protected int GetPublicItemCount()
        {
            return _data.Keys.Where(k => !k.StartsWith(_privateKeyPrefix)).Count();
        }

        public AsyncMessage() : base() { }

        public AsyncMessage(byte[] marshalledData) : base()
        {
            MarshalledData = marshalledData ?? throw new ArgumentNullException(nameof(marshalledData));
        }

        public void SetValue(string key, object value, bool overwriteDuplicates = false)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (key.StartsWith(_privateKeyPrefix))
            {
                throw new ArgumentException($"key can't start with '__'.  '{key}'");
            }

            if (!overwriteDuplicates && _data.ContainsKey(key))
            {
                throw new ArgumentException($"key already exists.  '{key}'");
            }

            _data[key] = value;
        }

        protected void InternalSetValue(string key, object value, bool overwriteDuplicates = false)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (!overwriteDuplicates && _data.ContainsKey(key))
            {
                throw new ArgumentException($"key already exists.  '{key}'");
            }

            _data[key] = value;
        }

        public object GetValue(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (key.StartsWith(_privateKeyPrefix))
            {
                throw new ArgumentException($"key can't start with '__'.  '{key}'");
            }

            if (!_data.TryGetValue(key, out object result))
            {
                throw new KeyNotFoundException(key);
            }

            return result;
        }

        protected object InternalGetValue(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (!_data.TryGetValue(key, out object result))
            {
                throw new KeyNotFoundException(key);
            }

            return result;
        }

        public T GetValue<T>(string key)
        {
            var result = GetValue(key);

            if (!(result is T))
            {
                throw new InvalidCastException($"the value of {key} is not of type {typeof(T).FullName}");
            }

            return (T)result;
        }

        public T InternalGetValue<T>(string key)
        {
            var result = InternalGetValue(key);

            if (!(result is T))
            {
                throw new InvalidCastException($"the value of {key} is not of type {typeof(T).FullName}");
            }

            return (T)result;
        }

        public string Path
        {
            get { return InternalGetValue<string>(_pathKey); }
            set { InternalSetValue(_pathKey, value, true); }
        }

        public string Id
        {
            get { return InternalGetValue<string>(_idKey); }
            set { InternalSetValue(_idKey, value, true); }
        }

        public string CorrelationId
        {
            get { return InternalGetValue<string>(_correlationIdKey); }
            set { InternalSetValue(_correlationIdKey, value, true); }
        }

        public DateTime TimeStamp
        {
            get { return InternalGetValue<DateTime>(_dateTimeStampKey); }
            set { InternalSetValue(_dateTimeStampKey, value, true); }
        }

        public byte[] MarshalledData
        {
            get
            {
                return Encoding.UTF8.GetBytes(Json);
            }
            protected set
            {
                if (value != null && value.Length > 0)
                {
                    var json = Encoding.UTF8.GetString(value, 0, value.Length);
                    _data = JsonConvert.DeserializeObject<Dictionary<string, object>>(json, _jsonSettings);
                }
                else
                {
                    _data = new Dictionary<string, object>();
                }
            }
        }

        public string Json => JsonConvert.SerializeObject(_data, _jsonSettings);
    }

    public sealed class AsyncRequest : AsyncMessage, IAsyncRequest
    {
        public AsyncRequest() : base() { }

        public AsyncRequest(byte[] marshalledData) : base(marshalledData) { }

        public AsyncRequest(MethodInfo methodInfo, object[] args) : base()
        {
            if (methodInfo == null)
            {
                throw new ArgumentNullException(nameof(methodInfo));
            }

            if (args != null)
            {
                var parameters = methodInfo.GetParameters();
                for (var i = 0; i < parameters.Length; ++i)
                {
                    InternalSetValue(parameters[i].Name, args[i]);
                }
            }

            Id = Guid.NewGuid().ToString();
            CorrelationId = Guid.NewGuid().ToString();
            Path = PathBuilder.BuildPath(methodInfo);
            TimeStamp = DateTime.UtcNow;
        }

        public int ArgumentCount => GetPublicItemCount();
    }

    public sealed class AsyncResponse : AsyncMessage, IAsyncResponse
    {
        private const string _returnValueKey = "__return_value";
        private const string _successKey = "__success";
        private const string _exceptionKey = "__exception";
        private const string _requestIdKey = "__request_id";

        //public AsyncResponse() : base()
        //{
        //    Exception = null;
        //    Success = true;
        //}

        public AsyncResponse(byte[] marshalledData) : base(marshalledData) { }

        public AsyncResponse(IAsyncRequest request, object value) : base()
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            // null might be a valid return value
            ReturnValue = value; // ?? throw new ArgumentNullException(nameof(value));

            Exception = null;
            Success = true;

            Initialize(request);
        }

        public AsyncResponse(IAsyncRequest request, Exception exception) : base()
        {
            Success = false;
            Exception = exception;
            ReturnValue = null;

            Initialize(request);
        }

        private void Initialize(IAsyncRequest request)
        {
            Id = Guid.NewGuid().ToString();
            TimeStamp = DateTime.UtcNow;

            RequestId = request.Id;
            CorrelationId = request.CorrelationId;
            Path = request.Path;
        }

        public bool Success
        {
            get { return (bool)InternalGetValue(_successKey); }
            private set { InternalSetValue(_successKey, value, true); }
        }

        public Exception Exception
        {
            get { return (Exception)InternalGetValue(_exceptionKey); }
            private set { InternalSetValue(_exceptionKey, value, true); }
        }

        public object ReturnValue
        {
            get { return InternalGetValue(_returnValueKey); }
            private set { InternalSetValue(_returnValueKey, value, true); }
        }

        public string RequestId
        {
            get { return (string)InternalGetValue(_requestIdKey); }
            private set { InternalSetValue(_requestIdKey, value, true); }
        }

        public T GetTypedReturnValue<T>()
        {
            return (T)ReturnValue;
        }
    }
}