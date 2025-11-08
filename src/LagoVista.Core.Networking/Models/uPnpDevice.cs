// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: fff68ca8cbba52653d4a0c9141e8b7fa0d065a6e4ba2605d1c287f7de3c68618
// IndexVersion: 2
// --- END CODE INDEX META ---
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LagoVista.Core.Networking.Models
{
    public class uPnPDevice
    {
        const String STR_NODE_NAMESPACE = "urn:schemas-upnp-org:device-1-0";

        public class Service
        {

            public Service(XElement ele)
            {
                ServiceType = GetNonNullField(ele.Element(XName.Get("serviceType", STR_NODE_NAMESPACE)));
                ServiceId = GetNonNullField(ele.Element(XName.Get("serviceId", STR_NODE_NAMESPACE)));
                ControlURL = GetNonNullField(ele.Element(XName.Get("controlURL", STR_NODE_NAMESPACE)));                
                EventSubURL = GetNonNullField(ele.Element(XName.Get("eventSubURL", STR_NODE_NAMESPACE)));
                SCPDURL = GetNonNullField(ele.Element(XName.Get("SCPDURL", STR_NODE_NAMESPACE)));
            }

            public Service()
            {

            }

            public String SCPDURL { get; set; }
            public String ServiceType { get; set; }
            public String ServiceId { get; set; }
            public String ControlURL { get; set; }
            
            public String EventSubURL { get; set; }
            public int ServiceIndex { get; set; } /* Used to determine port for server subscription */
        }

        public class Icon
        {
            public Icon(XElement ele)
            {
                MimeType = GetNonNullField(ele.Element(XName.Get("mimetype", STR_NODE_NAMESPACE)));
                var strWidth = GetNonNullField(ele.Element(XName.Get("width", STR_NODE_NAMESPACE)));
                var strHeight = GetNonNullField(ele.Element(XName.Get("height", STR_NODE_NAMESPACE)));
                int value = 48;
                if (int.TryParse(strWidth, out value))
                    Width = value;

                if (int.TryParse(strHeight, out value))
                    Height = value;

                Url = GetNonNullField(ele.Element(XName.Get("url", STR_NODE_NAMESPACE)));
            }

            public Icon()
            {

            }

            public String MimeType { get; set; }
            public int Width { get; set; }
            public int Height { get; set; }
            public String Url { get; set; }
        }

        public uPnPDevice()
        {

        }

        private static String GetNonNullField(XElement fld)
        {
            if (fld == null)
                return "Unknown";
            else
                return fld.Value;
        }        

        private uPnPDevice(XElement ele)
        {
            var udnParts = GetNonNullField(ele.Element(XName.Get("UDN", STR_NODE_NAMESPACE))).Split(':');

            if(udnParts.Count() == 2)
                UDN = udnParts[1];

            FriendlyName = GetNonNullField(ele.Element(XName.Get("friendlyName",STR_NODE_NAMESPACE)));
            ModelName = GetNonNullField(ele.Element(XName.Get("modelName", STR_NODE_NAMESPACE)));
            ModelNumber = GetNonNullField(ele.Element(XName.Get("modelNumber", STR_NODE_NAMESPACE)));
            ModelDescription = GetNonNullField(ele.Element(XName.Get("modelDescription", STR_NODE_NAMESPACE)));
            Manufacturer = GetNonNullField(ele.Element(XName.Get("manufacturer", STR_NODE_NAMESPACE)));
        }

    
        private static uPnPDevice Create(XDocument doc)
        {
            var deviceNode = from deviceNodes
                              in doc.Descendants()
                             where deviceNodes.Name.LocalName == "device"
                             select deviceNodes;

            var device = new uPnPDevice(deviceNode.FirstOrDefault());

            device.Services = (from serviceNode
                                in deviceNode.Descendants()
                               where serviceNode.Name.LocalName == "service"
                               select new Service(serviceNode)).ToList();

            var idx = 0;
            foreach(var service in device.Services)
                service.ServiceIndex = idx++;

            device.Icons = (from iconNode
                    in deviceNode.Descendants()
                               where iconNode.Name.LocalName == "icon"
                               select new Icon(iconNode)).ToList();
            return device;
        }

        public String UDN { get; set; }
        public String IPAddress { get; set; }
        public int Port { get; set; }
        public String ModelDescription { get; set; }
        public String FriendlyName { get; set; }
        public String ModelName { get; set; }
        public String ModelNumber { get; set; }

        public String Manufacturer { get; set; }

        public String Uri { get; set; }

        [JsonIgnore]
        public List<Service> Services { get; set; }

        public List<Icon> Icons { get; set; }

        public delegate void statusMethod(uPnPDevice device);

        [JsonIgnore]
        public Uri DeviceIcon
        {
            get
            {
                if (Icons.Count > 0)
                {
                    try
                    {
                        var imageUri = Icons.First().Url;
                        if (!imageUri.StartsWith("/"))
                            imageUri = "/" + imageUri;
                        var imagePath = String.Format("http://{0}:{1}{2}", IPAddress, Port, Icons.First().Url);
                        return new System.Uri(imagePath);
                    }
                    catch(Exception ex)
                    {
                        PlatformSupport.Services.Logger.AddException("uPnPDevice.DeviceIcon", ex);
                    }
                }

                return null;
            }
        }

        public static void GetDetails(String uri, statusMethod resultMethod)
        {
            try
            {
                Task.Run(async () => {
                    var client = new HttpClient();
                    //; charset = utf - 8
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("text/xml"));
                    using (var responseStream = await client.GetStreamAsync(uri))
                    {
                        var doc = XDocument.Load(responseStream);
                        Debug.WriteLine(doc.ToString());
                        var device = uPnPDevice.Create(doc);
                        var uriAddr = new Uri(uri);
                        device.IPAddress = uriAddr.Host;
                        device.Port = uriAddr.Port;

                        resultMethod(device);
                    }                   
                });
                /*
                var request = HttpWebRequest.CreateHttp(uri);
                request.Method = "GET";
                request.Accept = "text/xml; charset=utf-8";
                request.BeginGetResponse((asynchronousResult) =>
                {
                    var myWebRequest1 = (WebRequest)asynchronousResult.AsyncState;
                    // End the Asynchronous response.
                    using (var response = myWebRequest1.EndGetResponse(asynchronousResult))
                    using (var responseStream = response.GetResponseStream())
                    {
                        try
                        {
                     
                        }
                        catch(Exception)
                        {
                            resultMethod(null);
                        }
                    }
                }, request);*/
            }
            catch (Exception)
            {
                return;
            }
        }

        public override string ToString()
        {
            var bldr = new StringBuilder();
            bldr.AppendFormat("\r\n-------------------------------------------------------\r\n");
            bldr.AppendFormat(String.Format("Friendly Name:         \t: {0}\r\n", FriendlyName));
            bldr.AppendFormat(String.Format("Model Name:            \t: {0}\r\n", ModelName));
            bldr.AppendFormat(String.Format("Model Description:     \t: {0}\r\n", ModelDescription));
            bldr.AppendFormat(String.Format("Manufacturer:          \t: {0}\r\n", Manufacturer));
            bldr.AppendFormat(String.Format("IP Address:            \t: {0}\r\n", IPAddress));
            bldr.AppendFormat(String.Format("UDN:                   \t: {0}\r\n", UDN));
            bldr.AppendFormat("-------------------------------------------------------\r\n\r\n");

            return bldr.ToString();
        }
    }
}
