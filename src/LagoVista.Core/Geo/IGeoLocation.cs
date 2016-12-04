using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.Core.Geo
{
    public interface IGeoLocation
    {
        double Altitude { get; set; }
        string LastUpdated { get; set; }
        double Latitude { get; set; }
        double Longitude { get; set; }
    }
}
