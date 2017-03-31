using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.Models
{
    public class DependentObjectCheckResult
    {
        public bool IsInUse
        {
            get; private set;
        } = false;


        public IEnumerable<SummaryData> DependentObjects { get; }
    }
}
