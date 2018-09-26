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
