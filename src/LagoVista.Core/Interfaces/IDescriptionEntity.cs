// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 72c7f4adf184aa23157a59d87f1a44141ffce0e8eb6f13d7f8e5c968055a8655
// IndexVersion: 1
// --- END CODE INDEX META ---
using System;

namespace LagoVista.Core.Interfaces
{
    public interface IDescriptionEntity
    {
        String Description { get; set; }
    }

    public interface ITitledEntity
    {
        string Title { get; set; }
    }

    public interface IIconEntity
    {
        string Icon { get; set; }
    }
}
