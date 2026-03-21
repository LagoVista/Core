using LagoVista.Core.Validation;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LagoVista
{
    public class Error
    {
        [JsonProperty("hostId", NullValueHandling = NullValueHandling.Ignore)]
        public string HostId { get; set; }

        [JsonProperty("instanceId", NullValueHandling = NullValueHandling.Ignore)]
        public string InstanceId { get; set; }

        [JsonProperty("configurationType", NullValueHandling = NullValueHandling.Ignore)]
        public string ConfigurationType { get; set; }

        [JsonProperty("configurationId", NullValueHandling = NullValueHandling.Ignore)]
        public string ConfigurationId { get; set; }


        [JsonProperty("solutionId", NullValueHandling = NullValueHandling.Ignore)]
        public string SolutionId { get; set; }
        [JsonProperty("deviceConfigurationId", NullValueHandling = NullValueHandling.Ignore)]
        public string DeviceConfigurationId { get; set; }

        [JsonProperty("deviceId", NullValueHandling = NullValueHandling.Ignore)]
        public string DeviceId { get; set; }
        [JsonProperty("errorCode", NullValueHandling = NullValueHandling.Ignore)]
        public string ErrorCode { get; set; }
        [JsonProperty("message", NullValueHandling = NullValueHandling.Ignore)]
        public string Message { get; set; }
        [JsonProperty("details", NullValueHandling = NullValueHandling.Ignore)]
        public string Details { get; set; }
        [JsonProperty("extras", NullValueHandling = NullValueHandling.Ignore)]
        public List<KeyValuePair<string, string>> Extras { get; set; }

        public void SetEmptyValueToNull()
        {
            if (Extras != null && !Extras.Any()) Extras = null;
        }
    }

    public class ErrorCode
    {
        public string Code { get; set; }
        public string Message { get; set; }

        public Error ToError()
        {
            return new Error()
            {
                ErrorCode = Code,
                Message = Message
            };
        }

        public ErrorMessage ToErrorMessage(String details = "")
        {
            var result = new InvokeResult();
            var errorMessage = new ErrorMessage(Code, Message);
            errorMessage.Details = details;
            return errorMessage;
        }

        public InvokeResult ToFailedInvocation(String details = "")
        {
            var result = new InvokeResult();
            var errorMessage = new ErrorMessage(Code, Message);
            errorMessage.Details = details;
            result.Errors.Add(errorMessage);
            return result;
        }

        public InvokeResult<T> ToFailedInvocation<T>(String details = "")
        {
            var result = new InvokeResult<T>();
            var errorMessage = new ErrorMessage(Code, Message);
            errorMessage.Details = details;
            result.Errors.Add(errorMessage);
            return result;
        }
    }
}
