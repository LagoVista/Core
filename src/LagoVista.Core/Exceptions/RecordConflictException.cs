using System;

namespace LagoVista.Core.Exceptions
{
    /// <summary>
    /// Thrown when a mutation cannot be completed because the record has changed
    /// since the caller's expected version was read.
    /// </summary>
    public class RecordConflictException : Exception
    {
        public string RecordType { get; private set; }
        public string RecordId { get; private set; }
        public string ExpectedVersion { get; private set; }
        public string ActualVersion { get; private set; }

        public RecordConflictException(
            string recordType,
            string recordId,
            string expectedVersion = null,
            string actualVersion = null)
            : base(CreateMessage(recordType, recordId, expectedVersion, actualVersion))
        {
            RecordType = recordType;
            RecordId = recordId;
            ExpectedVersion = expectedVersion;
            ActualVersion = actualVersion;
        }

        private static string CreateMessage(
            string recordType,
            string recordId,
            string expectedVersion,
            string actualVersion)
        {
            var message = $"Record Of Type: {recordType} With Id: {recordId} Has A Version Conflict";

            if (!String.IsNullOrWhiteSpace(expectedVersion))
                message += $"; Expected Version: {expectedVersion}";

            if (!String.IsNullOrWhiteSpace(actualVersion))
                message += $"; Actual Version: {actualVersion}";

            return message;
        }
    }
}
