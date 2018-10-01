using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace LagoVista.Core.Rpc.Messages
{
    public class Message : IMessage
    {
        private const string _privateKeyPrefix = "__";
        private const string _pathKey = _privateKeyPrefix + "message_path";
        private const string _replyToPathKey = _privateKeyPrefix + "reply_path";
        private const string _idKey = _privateKeyPrefix + "message_id";
        private const string _correlationIdKey = _privateKeyPrefix + "correlation_id";
        private const string _dateTimeStampKey = _privateKeyPrefix + "datetimestamp_key";
        private const string _organizationId = _privateKeyPrefix + "organization_id";
        private const string _instanceId = _privateKeyPrefix + "instance_id";

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

        public Message() : base()
        {
            Id = string.Empty;
            CorrelationId = string.Empty;
            DestinationPath = string.Empty;
            InstanceId = string.Empty;
            OrganizationId = string.Empty;
            ReplyPath = string.Empty;
            TimeStamp = DateTime.UtcNow;
        }

        public Message(byte[] marshalledData) : base()
        {
            Payload = marshalledData ?? throw new ArgumentNullException(nameof(marshalledData));
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

            if (!_data.TryGetValue(key, out var result))
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

            if (!_data.TryGetValue(key, out var result))
            {
                throw new KeyNotFoundException(key);
            }

            return result;
        }

        public T GetValue<T>(string key)
        {
            var result = GetValue(key);
            if (result != null)
            {
                if (!(result is T))
                {
                    throw new InvalidCastException($"the value of {key} is not of type {typeof(T).FullName}");
                }
                return (T)result;
            }
            return default(T);
        }

        public T InternalGetValue<T>(string key)
        {
            var result = InternalGetValue(key);
            if (result != null)
            {
                if (!(result is T))
                {
                    throw new InvalidCastException($"the value of {key} is not of type {typeof(T).FullName}");
                }
                return (T)result;
            }
            return default(T);
        }

        public string DestinationPath
        {
            get => InternalGetValue<string>(_pathKey);
            set => InternalSetValue(_pathKey, value, true);
        }

        public string ReplyPath
        {
            get => InternalGetValue<string>(_replyToPathKey);
            set => InternalSetValue(_replyToPathKey, value, true);
        }

        public string Id
        {
            get => InternalGetValue<string>(_idKey);
            set => InternalSetValue(_idKey, value, true);
        }

        public string CorrelationId
        {
            get => InternalGetValue<string>(_correlationIdKey);
            set => InternalSetValue(_correlationIdKey, value, true);
        }

        public DateTime TimeStamp
        {
            get => InternalGetValue<DateTime>(_dateTimeStampKey);
            set => InternalSetValue(_dateTimeStampKey, value, true);
        }

        public string OrganizationId
        {
            get => InternalGetValue<string>(_organizationId);
            set => InternalSetValue(_organizationId, value, true);
        }
        public string InstanceId
        {
            get => InternalGetValue<string>(_instanceId);
            set => InternalSetValue(_instanceId, value, true);
        }

        [JsonIgnore]
        public byte[] Payload
        {
            get => Encoding.UTF8.GetBytes(Json);
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

        [JsonIgnore]
        public string Json => JsonConvert.SerializeObject(_data, _jsonSettings);
    }

    public class Request : Message, IRequest
    {
        public Request() : base() { }

        public Request(byte[] marshalledData) : base(marshalledData) { }

        public Request(MethodInfo methodInfo, object[] args, string organizationId, string instanceId, string replyPath) : base()
        {
            if (methodInfo == null)
            {
                throw new ArgumentNullException(nameof(methodInfo));
            }

            OrganizationId = organizationId ?? throw new ArgumentNullException(nameof(organizationId));
            InstanceId = instanceId ?? throw new ArgumentNullException(nameof(instanceId));
            ReplyPath = replyPath ?? throw new ArgumentNullException(nameof(replyPath));

            if (args != null)
            {
                var parameters = methodInfo.GetParameters();
                for (var i = 0; i < parameters.Length; ++i)
                {
                    InternalSetValue(parameters[i].Name, args[i]);
                }
                ValidateArguments(parameters, args);
            }

            Id = Guid.NewGuid().ToString();
            CorrelationId = Guid.NewGuid().ToString();
            DestinationPath = PathBuilder.BuildPath(methodInfo);
            TimeStamp = DateTime.UtcNow;
        }

        internal static void ValidateArguments(ParameterInfo[] parameters, object[] args)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            // 1. check request value count == param count
            if (args.Length != parameters.Length)
            {
                throw new ArgumentException($"parameter count mismatch. params {parameters.Length}, args {args.Length}.");
            }

            // 2. validate the types
            for (var i = 0; i < parameters.Length; ++i)
            {
                var parameter = parameters[i];

                if (parameter.GetCustomAttribute(typeof(ParamArrayAttribute)) != null)
                {
                    throw new NotSupportedException($"unsupported type - params keyword not allowed. type: '{parameter.Name}'.");
                }

                if (args[i] != null)
                {
                    var argType = args[i].GetType();
                    if (parameter.ParameterType != argType)
                    {
                        throw new ArgumentException($"parameter type mismatch. param: '{parameter.Name}', param type: '{parameter.ParameterType.FullName}', arg type: '{argType.FullName}'.");
                    }
                }
            }
        }

        [JsonIgnore]
        public int ArgumentCount => GetPublicItemCount();
    }

    public sealed class Response : Message, IResponse
    {
        private const string _returnValueKey = "__return_value";
        private const string _successKey = "__success";
        private const string _exceptionKey = "__exception";
        private const string _requestIdKey = "__request_id";

        public Response(byte[] marshalledData) : base(marshalledData) { }

        public Response(IRequest request, object value) : base()
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (value == null || !(value is Exception))
            {
                Success = true;
                ReturnValue = value;
                Exception = null;
            }
            else
            {
                Success = false;
                ReturnValue = null;
                Exception = (Exception)value;
            }

            Initialize(request);
        }

        private void Initialize(IRequest request)
        {
            Id = Guid.NewGuid().ToString();
            TimeStamp = DateTime.UtcNow;

            RequestId = request.Id;
            CorrelationId = request.CorrelationId;
            DestinationPath = request.DestinationPath;
            OrganizationId = request.OrganizationId;
            InstanceId = request.InstanceId;
            ReplyPath = request.ReplyPath;
        }

        public bool Success
        {
            get => InternalGetValue<bool>(_successKey);
            private set => InternalSetValue(_successKey, value, true);
        }

        public Exception Exception
        {
            get => InternalGetValue<Exception>(_exceptionKey);
            private set => InternalSetValue(_exceptionKey, value, true);
        }

        public object ReturnValue
        {
            get => InternalGetValue(_returnValueKey);
            private set => InternalSetValue(_returnValueKey, value, true);
        }

        public T GetTypedReturnValue<T>()
        {
            return InternalGetValue<T>(_returnValueKey);
        }

        public string RequestId
        {
            get => InternalGetValue<string>(_requestIdKey);
            private set => InternalSetValue(_requestIdKey, value, true);
        }
    }
}