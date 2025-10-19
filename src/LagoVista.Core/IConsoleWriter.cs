// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 511ce96eb5623451162201fef30df2fe5e6712d96670047e22a4e4511c5ec92f
// IndexVersion: 0
// --- END CODE INDEX META ---
using System;

namespace LagoVista.Core
{
    public interface IConsoleWriter
    {
        void WriteLine(string message);
        void WriteError(string message);
        void WriteWarning(string message);
    }

    public interface ILocalTime
    {
        DateTime Now { get; }
    }
}
