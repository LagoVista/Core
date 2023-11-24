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
