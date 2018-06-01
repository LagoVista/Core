using System;

namespace LagoVista.Core.Networking.AsyncMessaging
{
    public interface IAsyncMessage
    {
        void SetValue(string name, object value, bool ignoreDuplicates = false);
        T GetValue<T>(string name);
        object GetValue(string name);

        string Id { get; set; }
        string CorrelationId { get; set; }
        string OrganizationId { get; set; }
        string InstanceId { get; set; }
        string DestinationPath { get; set; }
        string ReplyPath { get; set; }
        DateTime TimeStamp { get; set; }

        byte[] Payload { get; }
        string Json { get; }

        //todo: ML - design and implement a versioning method for request and response
    }
}
