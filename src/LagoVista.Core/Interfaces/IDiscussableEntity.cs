// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: aebd8cedab53b0caf5f191393334b9230460c56e4a3652287942073a4a8a6b18
// IndexVersion: 1
// --- END CODE INDEX META ---
using LagoVista.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.Interfaces
{
    public interface IDiscussable
    {
        List<Discussion> Discussions { get; }
    }

    public interface IDiscussableEntity : IIDEntity, IDiscussable, INamedEntity, IOwnedEntity, IAuditableEntity
    {
    }
}
     