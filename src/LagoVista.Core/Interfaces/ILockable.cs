// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 94607048fce10e1c76b8c2a03028dfb8b0bc6316e775fd1d4d9258df7f38b1d0
// IndexVersion: 0
// --- END CODE INDEX META ---
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
