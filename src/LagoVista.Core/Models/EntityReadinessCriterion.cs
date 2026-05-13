using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LagoVista.Core.Models
{
    public class EntityReadinessCriterion
    {
        public string Key { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public List<string> RelatedFields { get; set; } = new List<string>();

        public bool IsRequired { get; set; }

        public bool IsBlockingCriterion { get; set; }

        public double Weight { get; set; } = 1.0;

        public static EntityReadinessCriterion Review(string key, string name, string description, IEnumerable<string> relatedFields = null, bool isRequired = true, bool isBlockingCriterion = false, double weight = 1.0)
        {
            return new EntityReadinessCriterion
            {
                Key = key,
                Name = name,
                Description = description,
                RelatedFields = relatedFields?.ToList() ?? new List<string>(),
                IsRequired = isRequired,
                IsBlockingCriterion = isBlockingCriterion,
                Weight = weight,
            };
        }

        public static List<EntityReadinessCriterion> Defaults()
        {
            return new List<EntityReadinessCriterion>
            {
                EntityReadinessCriterion.Review("identity-clarity", "Identity clarity", "The entity has a clear name, key, description, and other identity fields appropriate for its type.", new[] { "Name", "Key", "Description" }),

                EntityReadinessCriterion.Review("required-field-completeness", "Required field completeness", "Required and important fields appear populated with meaningful values, not placeholders or vague text.", Array.Empty<string>()),

                EntityReadinessCriterion.Review("internal-consistency", "Internal consistency", "The populated fields do not contradict each other and appear to describe the same record or concept.", Array.Empty<string>()),

                EntityReadinessCriterion.Review("configuration-validity", "Configuration validity", "The entity appears configured in a way that can be safely used by the system.", Array.Empty<string>()),

                EntityReadinessCriterion.Review("reviewability", "Reviewability", "A human or model can understand what this record is and whether it is correct without excessive guessing.", Array.Empty<string>()),
            };
        }
    }
}
