// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: cd76621ce36741fb0425ecaeab3c5e81b25a5ddef31cf47d8a28e98a787e66c0
// IndexVersion: 2
// --- END CODE INDEX META ---
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LagoVista.Core.Retry;
using System;
using System.Threading.Tasks;

namespace LagoVista.Core.Retry.Tests
{
    public class WithRetryTestClass
    {
        public Task<int> FunctionAsync(int x)
        {
            return Task.Run(() =>
            {
                if (x != 1)
                {
                    throw new Exception("boom");
                }

                return x;
            });
        }

        public int X { get; private set; }
        public void Action(int x)
        {
            X = x;
        }

        public int Function(int x)
        {
            return x;
        }

        public int Exception(int x)
        {
            throw new Exception("boom");
        }

        public Task<int> ExceptionAsync(int x)
        {
            return Task.Run(() =>
            {
                if (x != x + 1)
                {
                    throw new Exception("boom");
                }

                return x;
            });
        }

        public static T Invoke<T>(Func<T> action)
        {
            var isAwaitable = action.Method.ReturnType.GetMethod(nameof(Task.GetAwaiter)) != null;
            return (T)action.Method.Invoke(action.Target, null);
        }

        public static void Invoke(Action action)
        {
            action();
        }
    }

    [TestClass]
    public class WithRetry_Tests
    {
        [TestMethod]
        public async Task WithRetry_Invoke()
        {
            var ac = new WithRetryTestClass();
            WithRetry.SetDefaultOptions(new RetryOptions(3, TimeSpan.FromSeconds(15)));

            WithRetry.Invoke(() => ac.Action(1));

            var fr1 = WithRetry.Invoke(() => ac.Function(1));

            Assert.ThrowsExactly<NotSupportedException>(() => { var fr = WithRetry.Invoke(async () => await ac.FunctionAsync(1)); });

            await Assert.ThrowsExactlyAsync<NotSupportedException>(async () => { var fr = await WithRetry.Invoke(async () => await ac.FunctionAsync(1)); });

            Assert.ThrowsExactly<ExceededMaxAttemptsException>(() => { var fr = WithRetry.Invoke(() => ac.Exception(1)); });

            WithRetry.SetDefaultOptions(new RetryOptions(100000, TimeSpan.FromMilliseconds(10)));

            Assert.ThrowsExactly<ExceededMaxWaitTimeException>(() => { var fr = WithRetry.Invoke(() => ac.Exception(1)); });
        }

        [TestMethod]
        public async Task WithRetry_InvokeAsync()
        {
            var ac = new WithRetryTestClass();
            WithRetry.SetDefaultOptions(new RetryOptions(3, TimeSpan.FromSeconds(15)));

            var fr2 = await WithRetry.InvokeAsync(async () => await ac.FunctionAsync(1));

            await Assert.ThrowsExactlyAsync<ExceededMaxAttemptsException>(async () => { var fr = await WithRetry.InvokeAsync(() => ac.ExceptionAsync(1)); });

            WithRetry.SetDefaultOptions(new RetryOptions(100000, TimeSpan.FromMilliseconds(10)));

            await Assert.ThrowsExactlyAsync<ExceededMaxWaitTimeException>(async () => { var fr = await WithRetry.InvokeAsync(() => ac.ExceptionAsync(1)); });
        }
    }
}
