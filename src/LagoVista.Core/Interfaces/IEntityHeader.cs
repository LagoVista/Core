// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: f2c7abedbaa3d215bee1dfed02ee3ec946f413abe3f70c2fb4a9ed711b4e14b4
// IndexVersion: 0
// --- END CODE INDEX META ---
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LagoVista.Core.Interfaces
{
    public interface IEntityHeader
    {
        String Id { get; set; }
        String Text { get; set; }
        bool IsEmpty();

    }

    public interface IEntityHeader<T> : IEntityHeader 
    {
        T Value { get; set; }
        bool HasValue { get; }
    }
}
