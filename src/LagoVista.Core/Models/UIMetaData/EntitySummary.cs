﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static LagoVista.Core.Attributes.EntityDescriptionAttribute;

namespace LagoVista.Core.Models.UIMetaData
{
    public class EntitySummary
    {
        public String Name { get; set; }
        public String DomainKey { get; set; }
        public String ClassName { get; set; }

        public bool Cloneable { get; set; }
        public String ShortClassName { get; set; }

        public string UiCreateUrl { get; set; }
        public string UiGetListUrl { get; set; }
        public string UiEditUrl { get; set; }

        public String Icon { get; set; }
        public String Description { get; set; }

        public EntityTypes EntityType { get; set; }


        public static EntitySummary Create(EntityDescription desc)
        {
            return new EntitySummary()
            {
                ClassName = desc.Name,
                Name = desc.Title,
                DomainKey = desc.DomainName,
                Icon = desc.Icon,
                Description = desc.Description,
                EntityType = desc.EntityType,
                UiEditUrl = desc.EditUIUrl,
                UiCreateUrl = desc.CreateUIUrl,
                UiGetListUrl = desc.ListUIUrl,
                Cloneable = desc.Cloneable,
            };
        }
    }
}
