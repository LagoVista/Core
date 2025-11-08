// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: c58d0c37d0931bac91edcdadd1451a383134f01c0f34714ca8a00b8dcc1b5854
// IndexVersion: 2
// --- END CODE INDEX META ---
using System;

namespace LagoVista.Core.Interfaces
{
    public interface INoSQLEntity
    {
        String DatabaseName { get; set; }
        String EntityType { get; set; }
    }
}