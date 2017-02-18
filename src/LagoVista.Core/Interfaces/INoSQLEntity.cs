using System;

namespace LagoVista.Core.Interfaces
{
    public interface INoSQLEntity
    {
        String DatabaseName { get; set; }
        String EntityType { get; set; }
    }
}