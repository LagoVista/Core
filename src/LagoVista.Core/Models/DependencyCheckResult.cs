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


        public List<SummaryData> DependentObjects { get; }
    }
}
