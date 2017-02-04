using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LagoVista.Core.Interfaces
{
    public interface INoSQLEntity
    {
        String DatabaseName { get; set; }
        String EntityType { get; set; }
    }
}
