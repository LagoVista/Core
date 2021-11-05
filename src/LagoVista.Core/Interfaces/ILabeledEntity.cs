using LagoVista.Core.Models;
using System.Collections.Generic;

namespace LagoVista.Core.Interfaces
{
    public interface ILabeledEntity: IIDEntity, INamedEntity, IDescriptionEntity, INoSQLEntity
    {
        List<Label> Labels { get; set; }
    }
}
