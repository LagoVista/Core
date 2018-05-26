using LagoVista.Core.Networking.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
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

        private readonly JsonSerializerSettings _jsonSettings = new JsonSerializerSettings
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

        public AsyncMessage(Json json) : base()
        {
            if (json.HasValue)
            {
                MarshalledData = Encoding.UTF8.GetBytes(json);
            }
        }

        public AsyncMessage(byte[] marshalledData) : base()
        {
            MarshalledData = marshalledData ?? throw new ArgumentNullException(nameof(marshalledData));
        }

        public void SetValue(string key, object value, bool ignoreDuplicates = false)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (!ignoreDuplicates && _data.ContainsKey(key))
            {
                throw new ArgumentException($"key already exists {key}");
            }

            _data[key] = value;
        }

        public T GetValue<T>(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (!_data.TryGetValue(key, out object result))
            {
                throw new KeyNotFoundException(key);
            }

            if (!(result is T))
            {
                throw new InvalidCastException($"The value of {key} is not of type {typeof(T).FullName}");
            }

            return (T)result;
        }

        public object GetValue(string key)
        {
            if (!_data.TryGetValue(key, out object result))
                throw new KeyNotFoundException(key);

            return result;
        }

        public string Path
        {
            get { return GetValue<string>(_pathKey); }
            set { SetValue(_pathKey, value, true); }
        }

        public string Id
        {
            get { return GetValue<string>(_idKey); }
            set { SetValue(_idKey, value, true); }
        }

        public string CorrelationId
        {
            get { return GetValue<string>(_correlationIdKey); }
            set { SetValue(_correlationIdKey, value, true); }
        }

        public DateTime TimeStamp
        {
            get { return GetValue<DateTime>(_dateTimeStampKey); }
            set { SetValue(_dateTimeStampKey, value, true); }
        }

        public byte[] MarshalledData
        {
            get
            {
                var json = JsonConvert.SerializeObject(_data, _jsonSettings);
                return Encoding.UTF8.GetBytes(json);
            }
            set
            {
                if (value != null && value.Length > 0)
                {
                    var json = Encoding.UTF8.GetString(value, 0, value.Length);
                    _data = JsonConvert.DeserializeObject<Dictionary<string, object>>(json, _jsonSettings);
                }
            }
        }
    }

    public sealed class AsyncRequest : AsyncMessage, IAsyncRequest
    {
        public AsyncRequest() : base() { }

        public AsyncRequest(Json json) : base(json) { }

        public AsyncRequest(byte[] marshalledData) : base(marshalledData) { }

        public int ArgumentCount => GetPublicItemCount();
    }

    public class AsyncResponse : AsyncMessage, IAsyncResponse
    {
        private const string _responseKey = "__response";

        public AsyncResponse() : base() { }

        public AsyncResponse(Json json) : base(json) { }

        public AsyncResponse(byte[] marshalledData) : base(marshalledData) { }

        public AsyncResponse(object value) : base()
        {
            ReturnValue = value ?? throw new ArgumentNullException(nameof(value));
        }

        public AsyncResponse(Exception exception) : base()
        {
            Exception = exception;
            Success = false;
        }

        public bool Success { get; } = true;

        public Exception Exception { get; } = null;

        public object ReturnValue
        {
            get { return GetValue(_responseKey); }
            private set { SetValue(_responseKey, value, true); }
        }
    }

    public sealed class AsyncResponse<T> : AsyncResponse, IAsyncResponse<T>
    {
        public AsyncResponse() : base() { }

        public AsyncResponse(Json json) : base(json) { }

        public AsyncResponse(byte[] marshalledData) : base(marshalledData) { }

        public AsyncResponse(T value) : base(value) { }

        public AsyncResponse(Exception exception) : base(exception) { }

        public T TypedReturnValue { get { return (T)ReturnValue; } }
    }
}