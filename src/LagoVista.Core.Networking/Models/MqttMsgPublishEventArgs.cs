// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: dbe7e5e6ca5ae6d9ce0f94eb08bc07d9d4ee478a03992ff2d46e4bc03edefbf3
// IndexVersion: 2
// --- END CODE INDEX META ---
using LagoVista.Core.Networking.Interfaces;
using System;

namespace LagoVista.Core.Networking.Models
{
    public class MqttMsgPublishEventArgs : EventArgs
    {
        #region Properties...

        /// <summary>
        /// Message topic
        /// </summary>
        public string Topic
        {
            get; internal set;
        }

        /// <summary>
        /// Message data
        /// </summary>
        public byte[] Message
        {
            get; internal set;
        }

        /// <summary>
        /// Duplicate message flag
        /// </summary>
        public bool DupFlag
        {
            get; set;
        }

        /// <summary>
        /// Quality of Service level
        /// </summary>
        public QOS QosLevel
        {
            get; internal set;
        }

        /// <summary>
        /// Retain message flag
        /// </summary>
        public bool Retain
        {
            get; internal set;
        }

        public string MessageId
        {
            get; set;
        }

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="topic">Message topic</param>
        /// <param name="message">Message data</param>
        /// <param name="dupFlag">Duplicate delivery flag</param>
        /// <param name="qosLevel">Quality of Service level</param>
        /// <param name="retain">Retain flag</param>
        public MqttMsgPublishEventArgs(string topic,
            byte[] message,
            bool dupFlag,
            QOS qosLevel,
            bool retain)
        {
            Topic = topic;
            Message = message;
            DupFlag = dupFlag;
            QosLevel = qosLevel;
            Retain = retain;
        }
    }

}
