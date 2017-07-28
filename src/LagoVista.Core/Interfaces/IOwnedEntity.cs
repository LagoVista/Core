using LagoVista.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.Core.Interfaces
{
    public interface IOwnedEntity:IIDEntity
    {
        bool IsPublic { get; set; }
        EntityHeader OwnerOrganization { get; set; }
        EntityHeader OwnerUser { get; set; }
    }
}
