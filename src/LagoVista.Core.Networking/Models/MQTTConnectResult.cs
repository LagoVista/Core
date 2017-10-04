using LagoVista.Core.Networking.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.Networking.Models
{
    public class MQTTConnectResult
    {
        public MQTTConnectResult(ConnAck result, string message = "")
        {
            Result = result;
            Message = message;
        }

        public MQTTConnectResult(ConnAck result, Exception ex)
        {
            Result = result;
            Message = ex.Message;
        }

        public ConnAck Result { get; private set; }

        public string Message { get; private set; }
    }
}
