using LagoVista.Core.Attributes;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.Core.Tests.AutoMapper.TestModels
{
    public class SimpleWithEH
    {
        public string Name { get; set; }
        [MapFrom("ChildForEH")]
       public EntityHeader EntityEH { get; set; }
    }


    public class  SimpleWithEHDto
    {
        public string Name { get; set; }

        [IgnoreOnMapTo]
        public SimpleWithEHDtoChild ChildForEH { get; set; }
    }

    public class SimpleWithEHDtoChild : IEntityHeaderFactory
    {
        public string Id { get; set; }
        public string Key { get; set; }
        public string Name { get; set; }

        public string SomeOtherProp { get; set; }

        public string PropWeDonCareAbout { get; set; }

        public EntityHeader ToEntityHeader()
        {
            return EntityHeader.Create(Id, Key, Name);  
        }
    }
}
