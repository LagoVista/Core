// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 91a1b2ad751ac20a4a8151406cda2c901353987dda6fd3094a2969442b909c23
// IndexVersion: 0
// --- END CODE INDEX META ---
using LagoVista.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.Interfaces
{
    public interface ICategorized
    {
        EntityHeader Category { get; set; }
    }
}
