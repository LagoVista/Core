using LagoVista.Core.Models;
using System.Collections.Generic;

namespace LagoVista.Core.Interfaces
{
    public interface ILabeledEntity
    {
        List<Label> Labels { get; set; }
    }
}
