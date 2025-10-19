// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: a80f69cf6c8dff19686113c6e5841273c4611c7f89e35cadb76fb7e17bae8ab5
// IndexVersion: 0
// --- END CODE INDEX META ---
using System;

namespace LagoVista.Core.Rpc.Messages
{
    public interface IMessage
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
