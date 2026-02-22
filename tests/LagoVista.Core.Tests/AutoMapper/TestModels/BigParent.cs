using LagoVista.Core.Models;
using LagoVista.Models;
using System;
using System.Collections.Generic;

namespace LagoVista.Core.Tests.AutoMapper.TestModels
{
    public class BigParent : RelationalEntityBase
    {
    
        public Child1 Child1 { get; set; }
        public EntityHeader<Child2> Child2 { get; set; }
        public List<Child3> Child3 { get; set; }  
    }

    public class BigParentDTO : DbModelBase
    {

        public Child1DTO Child1 { get; set; }
        public Child2DTO Child2 { get; set; }   
        public List<Child3DTO> Child3 { get; set; }
    }


    public class Child1
    {
        public GrandChild1 GrandChild1 { get; set; }
        public EntityHeader<GrandChild2> GrandChild2 { get; set; }
    }

    public class Child1DTO
    {
        public GrandChild1DTO GrandChild1 { get; set; }
        public GrandChild2DTO GrandChild2 { get; set; }
    }

    public class GrandChild1
    {
        public string Name { get; set; }
    }

    public class GrandChild1DTO
    {
       public string Name { get; set; }
    }


    public class GrandChild2
    {
        public string LastSync { get; set; }
    }

    public class GrandChild2DTO
    {
        public DateTime LastSync { get; set; }

    }


    public class Child2
    {
        public int Amount { get; set; }
    }

    public class Child2DTO
    {
        public int Amount { get; set; }
    }

    public class Child3
    {
        public int Amount { get; set; }
    }

    public class Child3DTO
    {
        public int Amount { get; set; }
    }


}
