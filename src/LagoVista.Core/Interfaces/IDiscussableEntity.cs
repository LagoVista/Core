using LagoVista.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.Interfaces
{
    public interface IDiscussableEntity
    {
        List<Discussion> Discussions { get; } 
    }
}
