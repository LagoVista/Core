// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: db325d5b5196bc59e5d87a05660715ac28bf8ad41c22fd75246d960fde9489a7
// IndexVersion: 0
// --- END CODE INDEX META ---
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.Models
{
    public enum RDBMSConnectionType
    {
        SQLServerShared,
        SQLServerDedicated,
        PosgreSQLDedicated
    }

    public class RDBMSConnection
    {
        public RDBMSConnection(RDBMSConnectionType connectionType, string connectionString)
        {
            ConnectionType = connectionType;
            ConnectionString = connectionString;
        }

        public RDBMSConnection(RDBMSConnectionType connectionType)
        {
            ConnectionType = connectionType;
        }

        public RDBMSConnectionType ConnectionType { get; }
        public string ConnectionString { get; }
        
        public bool VerboseLogging { get; set; }
    }
}
