// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 27850b2763572f93ae1b5dbb3f6464d17d531eaa85344af43005ee68fddc0cae
// IndexVersion: 0
// --- END CODE INDEX META ---
using LagoVista.Core.Models;
using System.Collections.Generic;

namespace LagoVista.Core.Interfaces
{
    public interface ILabeledEntity: INoSQLEntity, IIDEntity, INamedEntity, IDescriptionEntity, IAuditableEntity
    {
        List<Label> Labels { get; set; }
    }

    public class LabeledEntity : EntityBase, ILabeledEntity
    {
    }
}
