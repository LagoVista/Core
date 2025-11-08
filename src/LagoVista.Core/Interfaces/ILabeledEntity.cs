// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: ce60e28450d145a0c9ca0058154b3c65cf701dd2cecc06211fcd6df860684bbc
// IndexVersion: 1
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
