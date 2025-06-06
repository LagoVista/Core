﻿using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.Interfaces
{
    public interface ISummaryData : IIDEntity, IKeyedEntity, INamedEntity, IIconEntity
    {
        bool IsPublic { get; }
        string Description { get; }
        bool? IsDeleted { get; }
    
        string Category { get; set; }
        string CategoryKey { get; set; }
        string CategoryId { get; set; }
    }

}
