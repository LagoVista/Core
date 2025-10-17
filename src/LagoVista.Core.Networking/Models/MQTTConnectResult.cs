// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 8282a9bd913f476c3327ad149622c79b7cc4a595c846dd99598fa931c40b2a2a
// IndexVersion: 1
// --- END CODE INDEX META ---
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
