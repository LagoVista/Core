using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.Interfaces
{
    public interface IVersionProvider
    {
        HostVersion GetHostVersion();
    }

    public class HostVersion
    {
        public string Version { get; set; }
        public string BuildDate { get; set; }
    }

}
