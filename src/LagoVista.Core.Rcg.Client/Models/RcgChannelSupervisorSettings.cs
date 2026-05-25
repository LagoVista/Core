using System;

namespace LagoVista.Core.Rcg.Client.Models
{
    public class RcgChannelSupervisorSettings
    {
        public int InitialReconnectDelaySeconds { get; set; }
        public int MaxReconnectDelaySeconds { get; set; }
        public int ConnectTimeoutSeconds { get; set; }
        public bool StopOnAuthorizationFailure { get; set; }

        public RcgChannelSupervisorSettings()
        {
            InitialReconnectDelaySeconds = 2;
            MaxReconnectDelaySeconds = 60;
            ConnectTimeoutSeconds = 30;
            StopOnAuthorizationFailure = true;
        }

        public TimeSpan GetInitialReconnectDelay()
        {
            return TimeSpan.FromSeconds(InitialReconnectDelaySeconds <= 0 ? 2 : InitialReconnectDelaySeconds);
        }

        public TimeSpan GetMaxReconnectDelay()
        {
            return TimeSpan.FromSeconds(MaxReconnectDelaySeconds <= 0 ? 60 : MaxReconnectDelaySeconds);
        }

        public TimeSpan GetConnectTimeout()
        {
            return TimeSpan.FromSeconds(ConnectTimeoutSeconds <= 0 ? 30 : ConnectTimeoutSeconds);
        }
    }
}
