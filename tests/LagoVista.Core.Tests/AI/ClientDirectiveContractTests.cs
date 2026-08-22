using LagoVista.Core.AI.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace LagoVista.Core.Tests.AI
{
    [TestFixture]
    public class ClientDirectiveContractTests
    {
        [Test]
        public void ClientDirectiveContinuation_WithOneResult_ShouldValidate()
        {
            var request = CreateDirectiveContinuation();
            Assert.DoesNotThrow(() => request.Validate());
        }

        [Test]
        public void ClientDirectiveContinuation_WithInstruction_ShouldFail()
        {
            var request = CreateDirectiveContinuation();
            request.Instruction = "continue normally";
            Assert.Throws<InvalidOperationException>(() => request.Validate());
        }

        [Test]
        public void ClientDirectiveContinuation_WithToolResults_ShouldFail()
        {
            var request = CreateDirectiveContinuation();
            request.ToolResults.Add(new ToolResultSubmission { ToolCallId = "tool-1", ResultJson = "{}" });
            Assert.Throws<InvalidOperationException>(() => request.Validate());
        }

        [Test]
        public void ClientDirectiveContinuation_WithMultipleResults_ShouldFail()
        {
            var request = CreateDirectiveContinuation();
            request.ClientDirectiveResults.Add(CreateDirectiveResult("directive-2"));
            Assert.Throws<InvalidOperationException>(() => request.Validate());
        }

        [Test]
        public void ClientDirectiveResult_WithScalarAndMultiSelect_ShouldFail()
        {
            var result = CreateDirectiveResult("directive-1");
            result.Scalar = new ClientDirectiveScalarValue { StringValue = "A" };
            result.MultiSelect = new ClientDirectiveMultiSelectValue { StringValues = new List<string> { "A", "B" } };
            Assert.Throws<InvalidOperationException>(() => result.Validate());
        }

        [Test]
        public void Scalar_WithExactlyOneTypedValue_ShouldValidate()
        {
            var scalar = new ClientDirectiveScalarValue { FlagValue = false };
            Assert.DoesNotThrow(() => scalar.Validate());
        }

        [Test]
        public void Scalar_WithMultipleTypedValues_ShouldFail()
        {
            var scalar = new ClientDirectiveScalarValue { StringValue = "A", NumberValue = 1 };
            Assert.Throws<InvalidOperationException>(() => scalar.Validate());
        }

        [Test]
        public void MultiSelect_WithHomogeneousStrings_ShouldValidate()
        {
            var value = new ClientDirectiveMultiSelectValue { StringValues = new List<string> { "A", "B" } };
            Assert.DoesNotThrow(() => value.Validate());
        }

        [Test]
        public void MultiSelect_WithStringsAndNumbers_ShouldFail()
        {
            var value = new ClientDirectiveMultiSelectValue { StringValues = new List<string> { "A" }, NumberValues = new List<decimal> { 1 } };
            Assert.Throws<InvalidOperationException>(() => value.Validate());
        }

        private static AgentExecuteRequest CreateDirectiveContinuation()
        {
            return new AgentExecuteRequest
            {
                SessionId = "session-1",
                TurnId = "turn-1",
                ClientDirectiveResults = new List<ClientDirectiveResult> { CreateDirectiveResult("directive-1") }
            };
        }

        private static ClientDirectiveResult CreateDirectiveResult(string directiveId)
        {
            return new ClientDirectiveResult
            {
                DirectiveId = directiveId,
                Action = "select_option",
                Result = "selected"
            };
        }
    }
}
