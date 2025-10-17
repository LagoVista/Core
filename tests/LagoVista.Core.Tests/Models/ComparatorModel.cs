// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 2e9067dd436a536c955c74daffee3702a54ebcf397dff9ab78b24a0d39da6fdf
// IndexVersion: 1
// --- END CODE INDEX META ---
using LagoVista.Core.Attributes;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using LagoVista.Core.Tests.Resources.Models;
using System;
using System.Collections.Generic;

namespace LagoVista.Core.Tests.Models
{
    public class ComparatorModelSimple : IIDEntity
    {
        public String Id { get; set; }


        [FormField(LabelResource: ValidationResources.Names.FirstNameLabel, ResourceType: typeof(ValidationResources))]
        public String FirstName { get; set; }

        public int Value1 { get; set; }
        public int? NullableValue { get; set; }
    }

    public class ComparatorModelModerate : ComparatorModelSimple, IIDEntity
    {
        public String LastName { get; set; }

        public EntityHeader EntityHeaderField  { get; set; }
    }


    public class ComparatorModelComplex : ComparatorModelModerate, IIDEntity
    {
     
        public ComparatorModelComplex()
        {
            ListItems = new List<EntityHeader>();
        }

        public List<EntityHeader> ListItems { get; private set; }

        public ComparatorModelModerate Parent { get; private set; }
    }

    public class ComparatorModelDeepGraph : ComparatorModelComplex, IIDEntity
    {
        public ComparatorModelComplex Grandparent { get; private set; }
    }

}
