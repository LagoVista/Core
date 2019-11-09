using LagoVista.Core.Models;
using System;

namespace LagoVista.Core.Interfaces
{
    /* Can be attached to an Entity that will allow it to be locked so it needs to be unlocked by an org admin before it can be edited */
    public interface ILockable
    {
        bool IsLocked { get; set; }

        EntityHeader LockedBy { get; set; }

        String LockedDateStamp { get; set; }
    }
}
