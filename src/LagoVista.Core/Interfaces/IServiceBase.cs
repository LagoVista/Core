// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: a053a06e2b4b237eeab1f6120a1968e03943c799c4653a2a2a587eb66e76fe00
// IndexVersion: 1
// --- END CODE INDEX META ---
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.Core.Interfaces
{
    public interface IServiceBase
    {
        bool IsConnected { get; }

        DateTime? DateConnected { get; }
        DateTime? DateLastMessage { get; }
        DateTime? DateDisconnected { get; }

        bool ShowDiagnostics { get; set; }

        IConnectionSettings ConnectionSettings { get; }

        Task InitAsync();
        Task SaveCredentialsAsync();

        event EventHandler Connected;
        event EventHandler Disconnected;

        /// <summary>
        /// Set this to true to ensure that when message come in they are forced to the main thread.
        /// this is helpeful for data binding.
        /// </summary>
        bool PropertiesUpdatedOnMainThread { get; set; }

    }
}
