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
