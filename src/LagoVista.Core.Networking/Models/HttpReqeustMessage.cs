// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: cfe458cd1f6e5c2fed454538b904c9f42673caee00cd8a541b4cbc4689225ae4
// IndexVersion: 1
// --- END CODE INDEX META ---
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.Core.Networking.Models
{
    public class HttpRequestMessage
    {
        public String Method { get; private set; }
        public String Body { get; private set; }
        public String Accept { get; private set; }
        public String ContentType { get; private set; }
        public int ContentLength { get; private set; }
        public String AcceptLanguage { get; private set; }
        public String AcceptEncoding { get; private set; }
        public String UserAgent { get; private set; }
        public String Host { get; private set; }
        public String Connection { get; private set; }
        public String Path { get; private set; }
        public Stream ResponseStream { get; private set; }

        public Dictionary<string, string> HeaderValues { get; private set; }

        public static HttpRequestMessage Create(Stream output, String requestData)
        {
            var msg = new HttpRequestMessage();
            msg.HeaderValues = new Dictionary<string, string>();
            msg.ResponseStream = output;

            var requestMethod = requestData.ToString().Split('\n')[0];
            var requestParts = requestMethod.Split(' ');
            msg.Method = requestParts[0];
            var headerMarker = requestData.ToString().IndexOf("\r\n\r\n") + 4;
            Debug.WriteLine(" HEADER => {0}\r\nLEN {1} {2}", requestData, headerMarker, requestData.Length);
            var header = requestData.Substring(0, headerMarker);
            var headerLines = header.Split('\n');

            for (var idx = 1; idx < headerLines.Length; ++idx)
            {
                var headerLine = headerLines[idx].TrimEnd('\r');
                if (!String.IsNullOrEmpty(headerLine))
                { 
                    var headerParts = headerLine.Split(':');
                    var headerField = headerParts[0];
                    var headerValue = headerParts[1].Trim();

                    switch(headerField.ToLower())
                    {
                        case "accept": msg.Accept = headerValue; break;
                        case "accept-language": msg.AcceptLanguage = headerValue; break;
                        case "accept-encoding": msg.AcceptEncoding = headerValue; break;
                        case "user-agent": msg.UserAgent = headerValue; break;
                        case "host": msg.Host = headerValue; break;
                        case "connection": msg.Connection = headerValue; break;
                        case "content-type": msg.ContentType = headerValue; break;
                        case "content-length": msg.ContentLength = Convert.ToInt32(headerValue); break;
                    }

                    msg.HeaderValues.Add(headerField, headerValue);
                }
            }

            if (msg.ContentLength > 0)
            {
                if(requestData.Length < (headerMarker + msg.ContentLength))
                {
                    if(requestData.Length < (headerMarker + msg.ContentLength - 1))
                        Debug.WriteLine(String.Format("POSSIBLE CORREUPTED MESSAGE Content Length {0} - Actual Body Length {1}\r\n{2}", requestData.Length,headerMarker, requestData));
                    msg.Body = requestData.ToString().Substring(headerMarker, msg.ContentLength - 1);
                    Debug.WriteLine(msg.Body);
                }
                else
                    msg.Body = requestData.ToString().Substring(headerMarker, msg.ContentLength);
            }

            msg.Path = requestParts[1];
         
            return msg;
        }


        public HttpResponseMessage GetResponseMessage()
        {
            return new HttpResponseMessage(ResponseStream);
        }

        public HttpResponseMessage GetErrorMessage(Exception ex, String message = "")
        {
            var responseMessage = new HttpResponseMessage(ResponseStream);
            responseMessage.ResponseCode = 500;
            responseMessage.ContentType = "text/html";


            var html = new StringBuilder();
            html.AppendLine("<html>");
            html.AppendLine("<head>");
            html.AppendLine("<title>500 Error</title>");
            html.AppendLine("</head>");
            html.AppendLine("<body style='font-family:consolas'>");
            html.AppendLine(ex.Message + "<br />");
            html.AppendLine("<div>");
            html.AppendLine(ex.StackTrace.Replace("\r","<br/>"));
            html.AppendLine("</div>");
            html.AppendLine("</body>");
            html.AppendLine("</html>");

            responseMessage.Content = html.ToString();


            return responseMessage;
        }

        public HttpResponseMessage<T> GetResponseMessage<T>()
        {
            return new HttpResponseMessage<T>(ResponseStream);
        }

        public override string ToString()
        {
            var bldr = new StringBuilder();
            bldr.AppendLine(String.Format("Method               \t: {0}", Method));
            bldr.AppendLine(String.Format("Path                 \t: {0}", Path));
            bldr.AppendLine(String.Format("Content Type         \t: {0}", ContentType));
            bldr.AppendLine(String.Format("Content Length       \t: {0}", ContentLength));
            bldr.AppendLine(String.Format("Accept               \t: {0}", Accept));
            bldr.AppendLine(String.Format("Accept-Language      \t: {0}", AcceptLanguage));
            bldr.AppendLine(String.Format("Accept-Encoding      \t: {0}", AcceptEncoding));
            bldr.AppendLine(String.Format("UserAgent            \t: {0}", UserAgent));
            bldr.AppendLine(String.Format("Connection           \t: {0}", Connection));
            if (String.IsNullOrEmpty(Body))
                bldr.AppendLine("No posted content");
            else
            {
                bldr.AppendLine("Body");
                bldr.AppendLine("---------------");
                bldr.AppendLine(Body);
            }

            bldr.ToString();

            return bldr.ToString();
        }
    }
}
