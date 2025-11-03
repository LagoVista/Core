using LagoVista.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.Interfaces
{
    public interface IAIIndexedEntity
    {
        string Id { get; }
        EntityHeader OwnerOrganization { get; }
        string GetHeader();
        string GetBodies();
        int Priority { get; set; }


    }
}
