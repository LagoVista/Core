using LagoVista.Core.Interfaces;
using LagoVista.Core.Managers;
using LagoVista.Core.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LagoVista.Core.Tests
{
    [TestClass]
    public class ProcessManagerTests
    {
        [TestMethod]
        public async Task StartAndTransitionUseDefinitionState()
        {
            var defs = new DefRepo { Item = CreateDefinition() };
            var inst = new InstRepo();
            var mgr = new ProcessManager(defs, inst);
            var runtime = await mgr.StartAsync(defs.Item.Id.Value);
            Assert.AreEqual("start", runtime.CurrentStateId);
            Assert.AreEqual(defs.Item.Revision, runtime.DefinitionRevision);
            runtime = await mgr.TransitionAsync(runtime.Id.Value, "finish");
            Assert.AreEqual("done", runtime.CurrentStateId);
            Assert.AreEqual(ProcessInstanceStatus.Completed, runtime.Status);
        }

        [TestMethod]
        public async Task StartAndTransitionRecordAuthoritativeRuntimeHistory()
        {
            var defs = new DefRepo { Item = CreateDefinition() };
            var inst = new InstRepo();
            var mgr = new ProcessManager(defs, inst);

            var runtime = await mgr.StartAsync(defs.Item.Id.Value);
            Assert.IsNotNull(runtime.RuntimeHistory);
            Assert.IsNotNull(runtime.ActivityHistory);
            Assert.IsNotNull(runtime.Telemetry);
            Assert.IsNotNull(runtime.Continuation);
            Assert.AreEqual(1, runtime.RuntimeHistory.Count);
            Assert.AreEqual("start", runtime.RuntimeHistory[0].EventType);
            Assert.AreEqual("start", runtime.Continuation.CurrentStateId);

            runtime = await mgr.TransitionAsync(runtime.Id.Value, "finish");
            Assert.AreEqual(2, runtime.RuntimeHistory.Count);
            Assert.AreEqual("transition", runtime.RuntimeHistory[1].EventType);
            Assert.AreEqual("finish", runtime.RuntimeHistory[1].TransitionId);
            Assert.AreEqual("start", runtime.RuntimeHistory[1].SourceStateId);
            Assert.AreEqual("done", runtime.RuntimeHistory[1].TargetStateId);
            Assert.AreEqual("finish", runtime.Continuation.LastTransitionId);
            Assert.AreEqual("done", runtime.Continuation.CurrentStateId);
            Assert.AreEqual(ProcessInstanceStatus.Completed, runtime.Continuation.Status);
            Assert.IsTrue(runtime.Telemetry.ElapsedMilliseconds >= 0);
        }

        [TestMethod]
        public async Task PauseBlocksTransitionUntilResume()
        {
            var defs = new DefRepo { Item = CreateDefinition() };
            var inst = new InstRepo();
            var mgr = new ProcessManager(defs, inst);
            var runtime = await mgr.StartAsync(defs.Item.Id.Value);
            await mgr.PauseAsync(runtime.Id.Value);
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => mgr.TransitionAsync(runtime.Id.Value, "finish"));
            await mgr.ResumeAsync(runtime.Id.Value);
            await mgr.TransitionAsync(runtime.Id.Value, "finish");
        }

        [TestMethod]
        public async Task PauseResumeTracksContinuationAndWaitingTime()
        {
            var defs = new DefRepo { Item = CreateDefinition() };
            var inst = new InstRepo();
            var mgr = new ProcessManager(defs, inst);
            var runtime = await mgr.StartAsync(defs.Item.Id.Value);

            runtime = await mgr.PauseAsync(runtime.Id.Value);
            Assert.AreEqual(ProcessInstanceStatus.Paused, runtime.Continuation.Status);
            Assert.IsFalse(String.IsNullOrWhiteSpace(runtime.Continuation.PausedAtUtc));
            Assert.AreEqual("pause", runtime.RuntimeHistory[runtime.RuntimeHistory.Count - 1].EventType);

            runtime.Continuation.PausedAtUtc = DateTime.UtcNow.AddSeconds(-2).ToString("o");
            runtime = await mgr.ResumeAsync(runtime.Id.Value);

            Assert.AreEqual(ProcessInstanceStatus.Running, runtime.Continuation.Status);
            Assert.IsNull(runtime.Continuation.PausedAtUtc);
            Assert.IsTrue(runtime.Telemetry.WaitingMilliseconds >= 1000);
            Assert.AreEqual("resume", runtime.RuntimeHistory[runtime.RuntimeHistory.Count - 1].EventType);
        }

        [TestMethod]
        public async Task GetInstanceNormalizesLegacyObservabilityFields()
        {
            var defs = new DefRepo { Item = CreateDefinition() };
            var inst = new InstRepo();
            var mgr = new ProcessManager(defs, inst);
            var legacy = new ProcessInstance
            {
                DefinitionId = defs.Item.Id.Value,
                DefinitionRevision = defs.Item.Revision,
                CurrentStateId = "start",
                Status = ProcessInstanceStatus.Running,
                StartedAtUtc = DateTime.UtcNow.AddMinutes(-1).ToString("o"),
                UpdatedAtUtc = DateTime.UtcNow.ToString("o"),
                RuntimeHistory = null,
                ActivityHistory = null,
                Telemetry = null,
                Continuation = null
            };
            await inst.AddAsync(legacy);

            var runtime = await mgr.GetInstanceAsync(legacy.Id.Value);
            Assert.IsNotNull(runtime.RuntimeHistory);
            Assert.IsNotNull(runtime.ActivityHistory);
            Assert.IsNotNull(runtime.Telemetry);
            Assert.IsNotNull(runtime.Continuation);
            Assert.IsNotNull(runtime.Continuation.Values);
            Assert.AreEqual("start", runtime.Continuation.CurrentStateId);
            Assert.AreEqual(ProcessInstanceStatus.Running, runtime.Continuation.Status);
            Assert.IsTrue(runtime.Telemetry.ElapsedMilliseconds >= 0);
        }

        [TestMethod]
        public async Task TransitionRejectsIllegalSourceState()
        {
            var defs = new DefRepo { Item = CreateDefinition() };
            var inst = new InstRepo();
            var mgr = new ProcessManager(defs, inst);
            var runtime = await mgr.StartAsync(defs.Item.Id.Value);
            runtime.CurrentStateId = "done";
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => mgr.TransitionAsync(runtime.Id.Value, "finish"));
        }

        private static ProcessDefinition CreateDefinition() => new ProcessDefinition
        {
            InitialStateId = "start",
            States = new List<ProcessState> { new ProcessState { Id = "start" }, new ProcessState { Id = "done", IsTerminal = true } },
            Transitions = new List<ProcessTransition> { new ProcessTransition { Id = "finish", SourceStateId = "start", TargetStateId = "done" } }
        };

        private sealed class DefRepo : IProcessDefinitionRepository
        {
            public ProcessDefinition Item { get; set; }
            public Task AddAsync(ProcessDefinition item) { Item = item; return Task.CompletedTask; }
            public Task<ProcessDefinition> GetAsync(string id) => Task.FromResult(Item != null && Item.Id.Value == id ? Item : null);
            public Task UpdateAsync(ProcessDefinition item) { Item = item; return Task.CompletedTask; }
        }

        private sealed class InstRepo : IProcessInstanceRepository
        {
            private readonly Dictionary<string, ProcessInstance> _items = new Dictionary<string, ProcessInstance>();
            public Task AddAsync(ProcessInstance item) { _items[item.Id.Value] = item; return Task.CompletedTask; }
            public Task<ProcessInstance> GetAsync(string id) { _items.TryGetValue(id, out var item); return Task.FromResult(item); }
            public Task UpdateAsync(ProcessInstance item) { _items[item.Id.Value] = item; return Task.CompletedTask; }
        }
    }
}
