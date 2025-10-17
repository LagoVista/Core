// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: ee5afc59d94f19241de88e910b41ad70d1a8b52cc1e6a4e54e91fc5b632f15d0
// IndexVersion: 1
// --- END CODE INDEX META ---
using System;
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
