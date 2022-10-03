using LagoVista.Core.Rpc.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.Core.Rpc.Tests.Server.Utils
{
    public class SampleMessage : IMessage
    {
        public string Id { get; set; }
        public string CorrelationId { get; set; }
        public string OrganizationId { get; set; }
        public string InstanceId { get; set; }
        public string DestinationPath { get; set; }
        public string ReplyPath { get; set; }
        public DateTime TimeStamp { get; set; }

        public byte[] Payload { get; set; }

        public string Json { get; set; }

        public T GetValue<T>(string name)
        {
            throw new NotImplementedException();
        }

        public object GetValue(string name)
        {
            throw new NotImplementedException();
        }

        public void SetValue(string name, object value, bool ignoreDuplicates = false)
        {
            throw new NotImplementedException();
        }
    }
}
