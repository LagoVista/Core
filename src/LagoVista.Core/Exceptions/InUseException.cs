using LagoVista.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.Exceptions
{
    /// <summary>
    /// Will be thrown if an attempt is made to delete an object that is in use.  The DependentObjects properties contains which objects are dependent.
    /// </summary>
    public class InUseException : Exception
    {
        public InUseException(Models.DependentObjectCheckResult result)
        {
            this.DependentObjects = result.DependentObjects;
        }

        public List<InUseRecordData> DependentObjects { get; set; }
    }
}
