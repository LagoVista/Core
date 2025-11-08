// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 6bfa1cdf80938f45a3e40f34930c5ade1f5b9e3e7aeffcb1786476de4ae935a6
// IndexVersion: 2
// --- END CODE INDEX META ---
using LagoVista.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.Interfaces
{
    public interface ICustomerOwnedEntity
    {
        EntityHeader Customer { get; set; }
    }
}
