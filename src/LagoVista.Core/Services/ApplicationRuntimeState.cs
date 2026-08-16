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
        private readonly AsyncLocal<int> _activeWorkDepth = new AsyncLocal<int>();
        private volatile ApplicationRuntimeState _state = ApplicationRuntimeState.Starting;
        private DateTime? _drainStartedUtc;
        private DateTime? _drainDeadlineUtc;

        public ApplicationRuntimeState State => _state;
        public bool IsDraining => _state == ApplicationRuntimeState.Draining || _state == ApplicationRuntimeState.Stopped;
        public bool HasActiveWorkContext => _activeWorkDepth.Value > 0;
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

            var workLease = CreateWorkLease(category, name, correlationId, longRunningCallback);
            if (workLease == null)
                return false;

            if (IsDraining)
            {
                workLease.Dispose();
                return false;
            }

            lease = workLease;
            return true;
        }

        public IDisposable BeginAdmittedWork(string category, string name, string correlationId, Action<ActiveApplicationWorkItem> longRunningCallback)
        {
            if (_state == ApplicationRuntimeState.Stopped)
                throw new InvalidOperationException("Application has stopped and cannot execute admitted work.");

            var lease = CreateWorkLease(category, name, correlationId, longRunningCallback);
            if (lease == null)
                throw new InvalidOperationException("Could not register admitted application work.");

            return lease;
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

        private IDisposable CreateWorkLease(string category, string name, string correlationId, Action<ActiveApplicationWorkItem> longRunningCallback)
        {
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
                return null;

            entry.StartLongRunningTimer(LongRunningThreshold);
            var contextLease = EnterWorkContext();
            return new WorkLease(this, snapshot.WorkId, contextLease);
        }

        private IDisposable EnterWorkContext()
        {
            var previousDepth = _activeWorkDepth.Value;
            _activeWorkDepth.Value = previousDepth + 1;
            return new WorkContextLease(_activeWorkDepth, previousDepth);
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
            private readonly IDisposable _contextLease;
            private int _disposed;

            public WorkLease(ApplicationRuntimeStateService owner, string workId, IDisposable contextLease)
            {
                _owner = owner;
                _workId = workId;
                _contextLease = contextLease;
            }

            public void Dispose()
            {
                if (Interlocked.Exchange(ref _disposed, 1) == 0)
                {
                    _contextLease?.Dispose();
                    _owner.CompleteWork(_workId);
                }
            }
        }

        private sealed class WorkContextLease : IDisposable
        {
            private readonly AsyncLocal<int> _depth;
            private readonly int _previousDepth;
            private int _disposed;

            public WorkContextLease(AsyncLocal<int> depth, int previousDepth)
            {
                _depth = depth;
                _previousDepth = previousDepth;
            }

            public void Dispose()
            {
                if (Interlocked.Exchange(ref _disposed, 1) == 0)
                    _depth.Value = _previousDepth;
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
