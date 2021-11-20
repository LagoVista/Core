using LagoVista.Core.Models;
using System.Collections.Generic;

namespace LagoVista.Core.Interfaces
{


    public interface ILabeledEntity: INoSQLEntity, IIDEntity, INamedEntity, IDescriptionEntity
    {
        List<Label> Labels { get; set; }
    }

    public class LabeledEntity : ILabeledEntity
    {
        public List<Label> Labels { get; set; }
        public string DatabaseName { get; set; }
        public string EntityType { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
