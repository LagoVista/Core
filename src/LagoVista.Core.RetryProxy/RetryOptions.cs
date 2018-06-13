namespace LagoVista.Core.Retry
{
    public class RetryOptions
    {
        public int MaxAttempts { get; set; }
        public int MaxWaitTimeInSeconds { get; set; }
    }
}
