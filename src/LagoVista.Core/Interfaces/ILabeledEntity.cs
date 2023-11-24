using LagoVista.Core.Models;
using System.Collections.Generic;

namespace LagoVista.Core.Interfaces
{


    public interface ILabeledEntity: INoSQLEntity, IIDEntity, INamedEntity, IDescriptionEntity, IAuditableEntity
    {
        List<Label> Labels { get; set; }
    }

    public class LabeledEntity : EntityBase
    {
        public List<Label> Labels { get; set; }
        public string Description { get; set; }
 }
}
