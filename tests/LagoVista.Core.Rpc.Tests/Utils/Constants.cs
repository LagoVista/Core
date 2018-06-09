using System;

namespace LagoVista.Core.Rpc.Tests.Utils
{
    public static class Constants
    {
        #region Messages
        public static readonly string MessageDestination = "Test.Destination.Path";
        public static readonly string MessageId = Guid.Parse("{C4CE5957-F9D7-4727-A20D-4C51AB5C6745}").ToString();
        public static readonly string MessageCorrelationId = Guid.Parse("{1C2FC03B-3D21-42A3-97F3-1756177DE2CB}").ToString();
        public static readonly string MessageReplyPath = "Test.ReplyTo.Path";
        public static readonly DateTime MessageTimeStamp = new DateTime(2018, 1, 1, 13, 30, 30);
        #endregion

        #region Common
        public static readonly string OrganizationId = Guid.Parse("{8AF59E47-E473-41D1-AA86-8B557813EEFB}").ToString();
        public static readonly string InstanceId = Guid.Parse("{EC0E2AE5-7B17-4C0D-9355-1903E3284FBE}").ToString();
        #endregion
    }
}
