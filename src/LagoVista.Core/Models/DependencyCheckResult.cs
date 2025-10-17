// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: afdd5bca9ed02916ae5a6c581e34ffaff0a137d81db0369c945cabcaa2615e53
// IndexVersion: 1
// --- END CODE INDEX META ---
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.Models
{
    public class DependentObjectCheckResult
    {
        public DependentObjectCheckResult()
        {
            DependentObjects = new List<InUseRecordData>();
        }

        public bool IsInUse
        {
          get { return DependentObjects.Count > 0; }
        }     

        public List<InUseRecordData> DependentObjects { get; private set; }

        public static DependentObjectCheckResult NotInUseResult()
        {
            return new DependentObjectCheckResult();
        }

        public static DependentObjectCheckResult InUse(params InUseRecordData[] inUseRecords)
        {
            var result = new DependentObjectCheckResult();
            foreach (var inUse in inUseRecords)
            {
                result.DependentObjects.Add(inUse);
            }

            return result;
        }
    }
}
