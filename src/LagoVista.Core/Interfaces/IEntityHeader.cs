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
