using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LagoVista.Core.Interfaces
{
    public enum ApplicationRuntimeState
    {
        Starting,
        Warming,
        Ready,
        Draining,
        Stopped
    }

    public sealed class ActiveApplicationWorkItem
    {
        public string WorkId { get; set; }
        public string Category { get; set; }
        public string Name { get; set; }
        public string CorrelationId { get; set; }
        public DateTime StartedUtc { get; set; }
        public TimeSpan Elapsed => DateTime.UtcNow - StartedUtc;
    }

    public interface IApplicationRuntimeState
    {
        ApplicationRuntimeState State { get; }
        bool IsDraining { get; }
        int ActiveWorkCount { get; }
        DateTime? DrainStartedUtc { get; }
        DateTime? DrainDeadlineUtc { get; }

        void MarkWarming();
        void MarkReady();
        void BeginDrain(TimeSpan drainTimeout);
        void MarkStopped();

        bool TryBeginWork(
            string category,
            string name,
            string correlationId,
            Action<ActiveApplicationWorkItem> longRunningCallback,
            out IDisposable lease);

        IReadOnlyCollection<ActiveApplicationWorkItem> GetActiveWork();
        Task<bool> WaitForIdleAsync(TimeSpan timeout, CancellationToken cancellationToken = default);
    }

    public static class ApplicationRuntimeStateProvider
    {
        public static IApplicationRuntimeState Instance { get; set; }
    }
}
