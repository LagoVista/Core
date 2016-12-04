using LagoVista.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.Core.Interfaces
{
    public interface IAuditableEntity
    {
        String CreationDate { get; set; }
        String LastUpdatedDate { get; set; }
        EntityHeader CreatedBy { get; set; }
        EntityHeader LastUpdatedBy { get; set; }
    }
}
