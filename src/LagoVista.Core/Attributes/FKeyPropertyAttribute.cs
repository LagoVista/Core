// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: f24d0588636da2c68781088bc7e8913885b0812d58781c3cd8b6552276a6a5f9
// IndexVersion: 2
// --- END CODE INDEX META ---
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class FKeyPropertyAttribute : Attribute
    {


        public FKeyPropertyAttribute(string PKeyObjectName, Type Type = null, string WhereClause = "", string UpdateClause = "")
        {
            this.PKeyObjectName = PKeyObjectName;
            this.WhereClause = WhereClause;
            this.UpdateClause = UpdateClause;
            this.FKeyType = Type;
        }

        public string PKeyObjectName { get;}
        public string WhereClause { get;}

        public string UpdateClause { get;}

        public Type FKeyType { get; }
    }
}
