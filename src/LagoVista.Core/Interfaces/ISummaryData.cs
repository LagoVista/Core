using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.Interfaces
{
    public interface ISummaryData : IIDEntity, IKeyedEntity, INamedEntity
    {
        bool IsPublic { get; }
        string Description { get; }
    }

}
