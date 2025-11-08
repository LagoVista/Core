// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 816cc2d37249b8a2e323f59494893b1b1e27ca4bda595c91fb5e6442779d6eae
// IndexVersion: 2
// --- END CODE INDEX META ---
using System;

namespace LagoVista.Core.Retry
{
    public class RetryOptions
    {
        public RetryOptions(int maxAttempts, TimeSpan timeout, TimeSpan? initialInterval = null)
        {
            if (maxAttempts < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxAttempts));
            }

            if (timeout.TotalMilliseconds <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(timeout));
            }

            MaxAttempts = maxAttempts;
            Timeout = timeout;
            InitialInterval = initialInterval ?? TimeSpan.FromMilliseconds(10);
        }

        public int MaxAttempts { get; }
        public TimeSpan Timeout { get; }
        public TimeSpan InitialInterval { get; }
    }
}
