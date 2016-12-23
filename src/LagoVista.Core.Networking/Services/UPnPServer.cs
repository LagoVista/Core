using LagoVista.Core.IOC;
using LagoVista.Core.Networking.Interfaces;
using LagoVista.Core.Networking.Models;
using LagoVista.Core.ServiceCommon;
using System;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.Core.Networking
{
    public abstract class SSDPDiscovery : ServiceBase
    {
        const uint BUFFER_SIZE = 8192;
        String _udn;
        IAPIServer _metaDataServer;
        INetworkListener _ssdpListener;

        private String GetDiscoveryPayload()
        {
            var config = GetSSDPConfig();

            var bldr = new StringBuilder();
            bldr.AppendLine("HTTP/1.1 200 OK");
            bldr.AppendLine(string.Format("ST: urn:schemas-upnp-org:device:{0}:1", config.DeviceType));
            bldr.AppendLine("CACHE-CONTROL: max-age=90");
            bldr.AppendLine("EXT");
            bldr.AppendLine(String.Format("USN: uuid:{0}::urn:schemas-upnp-org:device:{1}:1", _udn, config.DeviceType));
            bldr.AppendLine("SERVER: Win10IoT, UPnP/1.0");
            bldr.AppendLine(String.Format("LOCATION: http://{0}:{1}/xml/props.xml", PlatformSupport.Services.Network.GetIPV4Address(), GetMetaDataServerPort()));
            bldr.AppendLine("");

            return bldr.ToString();
        }



        protected async Task WriteResponseAsync(IStreamConnection connection, string contentType, int responseCode, String responseContent)
        {
            var header = String.Format("HTTP/1.1 {0} OK\r\n" +
                              "Content-Length: {1}\r\n" +
                              "Content-Type: {2}\r\n" +
                              "Connection: close\r\n\r\n",
                              responseCode, responseContent.Length, contentType);

            using (var writer = await connection.GetStreamWriterAsync())
            {
                await writer.WriteAsync(header);
                await writer.WriteAsync(responseContent);
            }
        }

        private const string DefaultPage = @"<html>
<head>
<title>Home Health Station Serivces</title>
</head>
<body>
Path not found.
</body>

</html>";

        public SSDPDiscovery()
        {
            /*     _udn = (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["UDN"];
                 if (String.IsNullOrEmpty(_udn))
                     _udn = Guid.NewGuid().ToString();

                 Windows.Storage.ApplicationData.Current.LocalSettings.Values["UDN"] = _udn;

                 _ssdpDiscoveryListener = new DatagramSocket();
                 _ssdpDiscoveryListener.MessageReceived += _socket_MessageReceived;

                 _webListener = new StreamSocketListener();
                 _webListener.ConnectionReceived += ProcessRequestAsync;*/
        }


        public async override Task InitAsync()
        {
            _ssdpListener = SLWIOC.Get<INetworkListener>();
            _ssdpListener.Start();
            _ssdpListener.ConnectionReceived += _ssdpListener_ConnectionReceived;

            _metaDataServer = SLWIOC.Get<IAPIServer>();

            _udn = await PlatformSupport.Services.Storage.GetKVPAsync<String>("UDN");
            if (String.IsNullOrEmpty(_udn))
            {
                _udn = Guid.NewGuid().ToString();
                await PlatformSupport.Services.Storage.StoreKVP("UDN", _udn);
            }

            await base.InitAsync();
        }

        private async void _ssdpListener_ConnectionReceived(object sender, IStreamConnection connection)
        {
            using (var writer = await connection.GetStreamWriterAsync())
            {
                await writer.WriteAsync(GetDiscoveryPayload());
            }
        }

        private async Task Process(IStreamConnection socket, String method, String path)
        {
            if (path.ToLower() == "/xml/props.xml")
            {
                await WriteResponseAsync(socket, "application/xml", 200, GetDeviceProps());
                return;
            }

            var rootDirectory = path.ToLower().Split('/');
            if (rootDirectory.Length == 0)
            {
                await WriteResponseAsync(socket, "text/html", 200, DefaultPage);
                return;
            }
        }

        public virtual async Task<bool> HandleRequestAsync(IStreamConnection connection, string path)
        {
            await WriteResponseAsync(connection, "text/html", 200, DefaultPage);
            return true;
        }

        protected abstract SSDPDiscoveryConfiguration GetSSDPConfig();

        protected abstract int GetMetaDataServerPort();

        private String GetDeviceProps()
        {
            var _config = GetSSDPConfig();

            String _deviceXML =
          @"<?xml version=""1.0""?>
<root xmlns=""urn:schemas-upnp-org:device-1-0"" >
    <specVersion >
    <major> 1 </major>
    <minor> 0 </minor>
    </specVersion >
 <device>
     <deviceType>urn:schemas-upnp-org:device:" + _config.DeviceType + @":1</deviceType>
     <presentationURL>/</presentationURL>
     <friendlyName>" + _config.FriendlyName + @"</friendlyName>
     <manufacturer>" + _config.Manufacture + @"</manufacturer>
     <manufacturerURL>" + _config.ManufactureUrl + @"</manufacturerURL>
     <modelDescription>" + _config.ModelDescription + @"</modelDescription>
     <modelName>" + _config.ModelName + @"</modelName>
     <modelNumber>" + _config.ModelNumber + @"</modelNumber>
     <modelURL>" + _config.ModelUrl + @"</modelURL>
     <serialNumber>" + _config.SerialNumber + @"</serialNumber>
     <UDN>uuid:" + _udn + @"</UDN>
     <serviceList>
        <service>
            <serviceType>urn:schemas-upnp-org:service:Dimming:1</serviceType>
            <serviceId>urn:upnp-org:serviceId:Dimming.0001</serviceId>
            <SCPDURL>_urn-upnp-org-serviceId-Dimming.0001_scpd.xml</SCPDURL>
            <controlURL>_urn-upnp-org-serviceId-Dimming.0001_control</controlURL>
            <eventSubURL>_urn-upnp-org-serviceId-Dimming.0001_event</eventSubURL>
        </service>
        <service>
            <serviceType>urn:schemas-upnp-org:service:SwitchPower:1</serviceType>
            <serviceId>urn:upnp-org:serviceId:SwitchPower.0001</serviceId>
            <SCPDURL>_urn-upnp-org-serviceId-SwitchPower.0001_scpd.xml</SCPDURL>
            <controlURL>_urn-upnp-org-serviceId-SwitchPower.0001_control</controlURL>
            <eventSubURL>_urn-upnp-org-serviceId-SwitchPower.0001_event</eventSubURL>
        </service>
    </serviceList>
 </device>
</root>";

            return _deviceXML;
        }


    }
}
