using LagoVista.Core.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LagoVista.Core.Services
{
    public sealed class ApplicationRuntimeStateService : IApplicationRuntimeState
    {
        private static readonly TimeSpan LongRunningThreshold = TimeSpan.FromMinutes(5);
        private readonly ConcurrentDictionary<string, ActiveEntry> _active = new ConcurrentDictionary<string, ActiveEntry>();
        private volatile ApplicationRuntimeState _state = ApplicationRuntimeState.Starting;
        private DateTime? _drainStartedUtc;
        private DateTime? _drainDeadlineUtc;

        public ApplicationRuntimeState State => _state;
        public bool IsDraining => _state == ApplicationRuntimeState.Draining || _state == ApplicationRuntimeState.Stopped;
        public int ActiveWorkCount => _active.Count;
        public DateTime? DrainStartedUtc => _drainStartedUtc;
        public DateTime? DrainDeadlineUtc => _drainDeadlineUtc;

        public ApplicationRuntimeStateService()
        {
            ApplicationRuntimeStateProvider.Instance = this;
        }

        public void MarkWarming()
        {
            if (_state == ApplicationRuntimeState.Starting)
                _state = ApplicationRuntimeState.Warming;
        }

        public void MarkReady()
        {
            if (!IsDraining)
                _state = ApplicationRuntimeState.Ready;
        }

        public void BeginDrain(TimeSpan drainTimeout)
        {
            if (drainTimeout <= TimeSpan.Zero)
                throw new ArgumentOutOfRangeException(nameof(drainTimeout));

            if (IsDraining)
                return;

            var now = DateTime.UtcNow;
            _drainStartedUtc = now;
            _drainDeadlineUtc = now.Add(drainTimeout);
            _state = ApplicationRuntimeState.Draining;
        }

        public void MarkStopped()
        {
            _state = ApplicationRuntimeState.Stopped;
        }

        public bool TryBeginWork(string category, string name, string correlationId, Action<ActiveApplicationWorkItem> longRunningCallback, out IDisposable lease)
        {
            lease = null;
            if (IsDraining)
                return false;

            var snapshot = new ActiveApplicationWorkItem
            {
                WorkId = Guid.NewGuid().ToString("N"),
                Category = category ?? String.Empty,
                Name = name ?? String.Empty,
                CorrelationId = correlationId ?? String.Empty,
                StartedUtc = DateTime.UtcNow
            };

            var entry = new ActiveEntry(snapshot, longRunningCallback);
            if (!_active.TryAdd(snapshot.WorkId, entry))
                return false;

            if (IsDraining)
            {
                ActiveEntry removed;
                _active.TryRemove(snapshot.WorkId, out removed);
                entry.Dispose();
                return false;
            }

            entry.StartLongRunningTimer(LongRunningThreshold);
            lease = new WorkLease(this, snapshot.WorkId);
            return true;
        }

        public IReadOnlyCollection<ActiveApplicationWorkItem> GetActiveWork()
        {
            return _active.Values
                .Select(entry => new ActiveApplicationWorkItem
                {
                    WorkId = entry.Snapshot.WorkId,
                    Category = entry.Snapshot.Category,
                    Name = entry.Snapshot.Name,
                    CorrelationId = entry.Snapshot.CorrelationId,
                    StartedUtc = entry.Snapshot.StartedUtc
                })
                .OrderBy(item => item.StartedUtc)
                .ToArray();
        }

        public async Task<bool> WaitForIdleAsync(TimeSpan timeout, CancellationToken cancellationToken = default)
        {
            if (timeout < TimeSpan.Zero)
                throw new ArgumentOutOfRangeException(nameof(timeout));

            var deadline = DateTime.UtcNow.Add(timeout);
            while (_active.Count > 0)
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (DateTime.UtcNow >= deadline)
                    return false;

                await Task.Delay(TimeSpan.FromMilliseconds(250), cancellationToken).ConfigureAwait(false);
            }

            return true;
        }

        private void CompleteWork(string workId)
        {
            ActiveEntry entry;
            if (_active.TryRemove(workId, out entry))
                entry.Dispose();
        }

        private sealed class WorkLease : IDisposable
        {
            private readonly ApplicationRuntimeStateService _owner;
            private readonly string _workId;
            private int _disposed;

            public WorkLease(ApplicationRuntimeStateService owner, string workId)
            {
                _owner = owner;
                _workId = workId;
            }

            public void Dispose()
            {
                if (Interlocked.Exchange(ref _disposed, 1) == 0)
                    _owner.CompleteWork(_workId);
            }
        }

        private sealed class ActiveEntry : IDisposable
        {
            private readonly Action<ActiveApplicationWorkItem> _longRunningCallback;
            private Timer _timer;
            private int _reported;

            public ActiveApplicationWorkItem Snapshot { get; }

            public ActiveEntry(ActiveApplicationWorkItem snapshot, Action<ActiveApplicationWorkItem> longRunningCallback)
            {
                Snapshot = snapshot;
                _longRunningCallback = longRunningCallback;
            }

            public void StartLongRunningTimer(TimeSpan threshold)
            {
                if (_longRunningCallback == null)
                    return;

                _timer = new Timer(_ =>
                {
                    if (Interlocked.Exchange(ref _reported, 1) == 0)
                        _longRunningCallback(Snapshot);
                }, null, threshold, Timeout.InfiniteTimeSpan);
            }

            public void Dispose()
            {
                _timer?.Dispose();
                _timer = null;
            }
        }
    }
}
