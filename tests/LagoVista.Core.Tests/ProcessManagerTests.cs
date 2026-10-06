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
