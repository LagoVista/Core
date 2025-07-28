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
     