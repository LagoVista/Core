using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace LagoVista.Core.Managers
{
    public sealed class ProcessManager : IProcessManager
    {
        private readonly IProcessDefinitionRepository _definitions;
        private readonly IProcessInstanceRepository _instances;

        public ProcessManager(IProcessDefinitionRepository definitions, IProcessInstanceRepository instances)
        {
            _definitions = definitions ?? throw new ArgumentNullException(nameof(definitions));
            _instances = instances ?? throw new ArgumentNullException(nameof(instances));
        }

        public async Task<ProcessDefinition> CreateDefinitionAsync(ProcessDefinition definition)
        {
            ValidateDefinition(definition);
            await _definitions.AddAsync(definition);
            return definition;
        }

        public Task<ProcessDefinition> GetDefinitionAsync(string id) => _definitions.GetAsync(id);

        public async Task<ProcessDefinition> UpdateDefinitionAsync(ProcessDefinition definition)
        {
            ValidateDefinition(definition);
            definition.Revision++;
            definition.RevisionTimeStamp = DateTime.UtcNow.ToString("o");
            await _definitions.UpdateAsync(definition);
            return definition;
        }

        public async Task<ProcessInstance> StartAsync(string definitionId)
        {
            var definition = await RequireDefinitionAsync(definitionId);
            ValidateDefinition(definition);
            var initial = definition.States.First(state => state.Id == definition.InitialStateId);
            var now = DateTime.UtcNow;
            var nowText = now.ToString("o");
            var instance = new ProcessInstance
            {
                DefinitionId = definition.Id.Value,
                DefinitionRevision = definition.Revision,
                CurrentStateId = initial.Id,
                Status = initial.IsTerminal ? ProcessInstanceStatus.Completed : ProcessInstanceStatus.Running,
                StartedAtUtc = nowText,
                UpdatedAtUtc = nowText
            };

            NormalizeRuntime(instance);
            instance.Continuation.CurrentStateId = instance.CurrentStateId;
            instance.Continuation.Status = instance.Status;
            instance.Continuation.LastUpdatedAtUtc = nowText;
            instance.Telemetry.ElapsedMilliseconds = 0;
            instance.RuntimeHistory.Add(new ProcessRuntimeHistoryEntry
            {
                EventType = "start",
                TimestampUtc = nowText,
                TargetStateId = instance.CurrentStateId,
                Status = instance.Status
            });

            await _instances.AddAsync(instance);
            return instance;
        }

        public async Task<ProcessInstance> GetInstanceAsync(string id)
        {
            var instance = await _instances.GetAsync(id);
            if (instance == null)
                return null;

            NormalizeRuntime(instance);
            return instance;
        }

        public async Task<ProcessInstance> TransitionAsync(string instanceId, string transitionId)
        {
            var instance = await RequireInstanceAsync(instanceId);
            if (instance.Status == ProcessInstanceStatus.Paused)
                throw new InvalidOperationException("A paused process instance cannot transition.");
            if (instance.Status == ProcessInstanceStatus.Completed)
                throw new InvalidOperationException("A completed process instance cannot transition.");

            var definition = await RequireDefinitionAsync(instance.DefinitionId);
            if (definition.Revision != instance.DefinitionRevision)
                throw new InvalidOperationException("The process instance definition revision no longer matches the persisted definition.");

            var transition = definition.Transitions.FirstOrDefault(item => item.Id == transitionId)
                ?? throw new InvalidOperationException("The requested process transition does not exist.");
            if (transition.SourceStateId != instance.CurrentStateId)
                throw new InvalidOperationException("The requested process transition is not legal from the current state.");
            var target = definition.States.FirstOrDefault(state => state.Id == transition.TargetStateId)
                ?? throw new InvalidOperationException("The requested process transition targets an unknown state.");

            var sourceStateId = instance.CurrentStateId;
            var now = DateTime.UtcNow;
            var nowText = now.ToString("o");
            instance.CurrentStateId = target.Id;
            instance.Status = target.IsTerminal ? ProcessInstanceStatus.Completed : ProcessInstanceStatus.Running;
            instance.UpdatedAtUtc = nowText;
            UpdateRuntime(instance, now);
            instance.Continuation.CurrentStateId = instance.CurrentStateId;
            instance.Continuation.Status = instance.Status;
            instance.Continuation.LastTransitionId = transition.Id;
            instance.Continuation.LastUpdatedAtUtc = nowText;
            instance.RuntimeHistory.Add(new ProcessRuntimeHistoryEntry
            {
                EventType = "transition",
                TimestampUtc = nowText,
                TransitionId = transition.Id,
                SourceStateId = sourceStateId,
                TargetStateId = target.Id,
                Status = instance.Status
            });

            await _instances.UpdateAsync(instance);
            return instance;
        }

        public async Task<ProcessInstance> PauseAsync(string instanceId)
        {
            var instance = await RequireInstanceAsync(instanceId);
            if (instance.Status != ProcessInstanceStatus.Running)
                throw new InvalidOperationException("Only a running process instance can be paused.");

            var sourceStateId = instance.CurrentStateId;
            var now = DateTime.UtcNow;
            var nowText = now.ToString("o");
            instance.Status = ProcessInstanceStatus.Paused;
            instance.UpdatedAtUtc = nowText;
            UpdateRuntime(instance, now);
            instance.Continuation.CurrentStateId = instance.CurrentStateId;
            instance.Continuation.Status = instance.Status;
            instance.Continuation.PausedAtUtc = nowText;
            instance.Continuation.LastUpdatedAtUtc = nowText;
            instance.RuntimeHistory.Add(new ProcessRuntimeHistoryEntry
            {
                EventType = "pause",
                TimestampUtc = nowText,
                SourceStateId = sourceStateId,
                TargetStateId = sourceStateId,
                Status = instance.Status
            });

            await _instances.UpdateAsync(instance);
            return instance;
        }

        public async Task<ProcessInstance> ResumeAsync(string instanceId)
        {
            var instance = await RequireInstanceAsync(instanceId);
            if (instance.Status != ProcessInstanceStatus.Paused)
                throw new InvalidOperationException("Only a paused process instance can be resumed.");

            var now = DateTime.UtcNow;
            var nowText = now.ToString("o");
            var pausedAt = ParseUtc(instance.Continuation.PausedAtUtc);
            if (pausedAt.HasValue && now > pausedAt.Value)
                instance.Telemetry.WaitingMilliseconds += (long)(now - pausedAt.Value).TotalMilliseconds;

            instance.Status = ProcessInstanceStatus.Running;
            instance.UpdatedAtUtc = nowText;
            UpdateRuntime(instance, now);
            instance.Continuation.CurrentStateId = instance.CurrentStateId;
            instance.Continuation.Status = instance.Status;
            instance.Continuation.PausedAtUtc = null;
            instance.Continuation.LastUpdatedAtUtc = nowText;
            instance.RuntimeHistory.Add(new ProcessRuntimeHistoryEntry
            {
                EventType = "resume",
                TimestampUtc = nowText,
                SourceStateId = instance.CurrentStateId,
                TargetStateId = instance.CurrentStateId,
                Status = instance.Status
            });

            await _instances.UpdateAsync(instance);
            return instance;
        }

        private async Task<ProcessDefinition> RequireDefinitionAsync(string id)
        {
            var definition = await _definitions.GetAsync(id);
            return definition ?? throw new InvalidOperationException("Process definition was not found.");
        }

        private async Task<ProcessInstance> RequireInstanceAsync(string id)
        {
            var instance = await _instances.GetAsync(id);
            if (instance == null)
                throw new InvalidOperationException("Process instance was not found.");

            NormalizeRuntime(instance);
            return instance;
        }

        private static void NormalizeRuntime(ProcessInstance instance)
        {
            if (instance.RuntimeHistory == null)
                instance.RuntimeHistory = new List<ProcessRuntimeHistoryEntry>();
            if (instance.ActivityHistory == null)
                instance.ActivityHistory = new List<ProcessActivityExecutionHistoryEntry>();
            if (instance.Telemetry == null)
                instance.Telemetry = new ProcessRuntimeTelemetry();
            if (instance.Continuation == null)
                instance.Continuation = new ProcessContinuationContext();
            if (instance.Continuation.Values == null)
                instance.Continuation.Values = new Dictionary<string, string>();

            instance.Continuation.CurrentStateId = instance.CurrentStateId;
            instance.Continuation.Status = instance.Status;
            if (String.IsNullOrWhiteSpace(instance.Continuation.LastUpdatedAtUtc))
                instance.Continuation.LastUpdatedAtUtc = instance.UpdatedAtUtc;

            var updatedAt = ParseUtc(instance.UpdatedAtUtc) ?? DateTime.UtcNow;
            UpdateRuntime(instance, updatedAt);
        }

        private static void UpdateRuntime(ProcessInstance instance, DateTime now)
        {
            var startedAt = ParseUtc(instance.StartedAtUtc);
            if (startedAt.HasValue && now >= startedAt.Value)
                instance.Telemetry.ElapsedMilliseconds = (long)(now - startedAt.Value).TotalMilliseconds;
        }

        private static DateTime? ParseUtc(string value)
        {
            if (String.IsNullOrWhiteSpace(value))
                return null;

            DateTime parsed;
            if (!DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out parsed))
                return null;

            return parsed.Kind == DateTimeKind.Utc ? parsed : parsed.ToUniversalTime();
        }

        private static void ValidateDefinition(ProcessDefinition definition)
        {
            if (definition == null) throw new ArgumentNullException(nameof(definition));
            if (definition.States == null || definition.States.Count == 0)
                throw new InvalidOperationException("A process definition must contain at least one state.");
            if (String.IsNullOrWhiteSpace(definition.InitialStateId) || !definition.States.Any(state => state.Id == definition.InitialStateId))
                throw new InvalidOperationException("A process definition must identify a valid initial state.");
            if (definition.States.Any(state => String.IsNullOrWhiteSpace(state.Id)) || definition.States.GroupBy(state => state.Id).Any(group => group.Count() > 1))
                throw new InvalidOperationException("Process state ids must be non-empty and unique.");
            foreach (var transition in definition.Transitions ?? Enumerable.Empty<ProcessTransition>())
            {
                if (String.IsNullOrWhiteSpace(transition.Id))
                    throw new InvalidOperationException("Process transition ids must be non-empty.");
                if (!definition.States.Any(state => state.Id == transition.SourceStateId) || !definition.States.Any(state => state.Id == transition.TargetStateId))
                    throw new InvalidOperationException("Every process transition must reference valid source and target states.");
            }
        }
    }
}
