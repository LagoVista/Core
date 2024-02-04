using LagoVista.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.Interfaces
{
    public interface ICategorized
    {
        EntityHeader Category { get; set; }
    }
}
