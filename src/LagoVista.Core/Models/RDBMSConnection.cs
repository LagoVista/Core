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
