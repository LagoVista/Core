// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: b2b7fcc4454d855cdf0d786abb9539c49831cd85dd02e15f05ccc118711aa538
// IndexVersion: 2
// --- END CODE INDEX META ---
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
