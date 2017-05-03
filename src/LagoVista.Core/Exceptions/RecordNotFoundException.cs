using LagoVista.Core.Validation;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.Exceptions
{
    public class RecordNotFoundException : Exception
    {
        public string RecordType { get; private set; }
        public string RecordId { get; private set; }

        public RecordNotFoundException(String recordType, string recordId)
        {
            RecordType = recordType;
            RecordId = recordId;
        }

        public InvokeResult ToFailedInvocation()
        {
            var result = new InvokeResult();
            var errMessage = new ErrorMessage($"Record Of Type: {RecordType} With Id: {RecordId}");
            result.Errors.Add(errMessage);
            return result;
        }
    }
}
