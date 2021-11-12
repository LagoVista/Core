using LagoVista.Core.Models;
using System.Collections.Generic;

namespace LagoVista.Core.Interfaces
{
    public interface ILabeledEntity: INoSQLEntity, IIDEntity, INamedEntity, IDescriptionEntity
    {
        List<Label> Labels { get; set; }
    }
}
