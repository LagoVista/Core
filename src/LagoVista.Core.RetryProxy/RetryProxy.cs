// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: e200e924a5568aa2a34cc927939b60989d34e43bd1be1710ba85644077e8339f
// IndexVersion: 0
// --- END CODE INDEX META ---
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace LagoVista.Core.Retry
{
    public class RetryProxy : DispatchProxy
    {
        public static TInterface Create<TInterface>(TInterface instance, RetryOptions options, IRetryExceptionTester retryExceptionTester = null, IEnumerable<Type> exceptionWhiteList = null, IEnumerable<Type> exceptionBlackList = null) where TInterface : class
        {
            var result = Create<TInterface, RetryProxy>();

            var proxy = (result as RetryProxy);
            proxy._instance = instance ?? throw new ArgumentNullException(nameof(instance));
            proxy._options = options ?? throw new ArgumentNullException(nameof(options));
            proxy._retryExceptionTester = retryExceptionTester;
            proxy._exceptionWhiteList = exceptionWhiteList;
            proxy._exceptionBlackList = exceptionBlackList;

            return result;
        }

        public static TInterface Create<TInterface, TImplementation>(RetryOptions options, IRetryExceptionTester retryExceptionTester = null, IEnumerable<Type> exceptionWhiteList = null, IEnumerable<Type> exceptionBlackList = null)
            where TInterface : class
            where TImplementation : class, new()
        {
            var instance = Activator.CreateInstance<TImplementation>() as TInterface;
            return Create(instance, options, retryExceptionTester, exceptionWhiteList, exceptionBlackList);
        }

        private object _instance;
        private RetryOptions _options;
        private IRetryExceptionTester _retryExceptionTester = null;
        private IEnumerable<Type> _exceptionWhiteList = null;
        private IEnumerable<Type> _exceptionBlackList = null;

        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            object result = null;

            List<Exception> exceptions = null;

            var waitTime = _options.InitialInterval;
            var currentAttempt = 1;
            Exception exception = null;

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var complete = false;
            while (!complete)
            {
                try
                {
                    var isAwaitable = targetMethod.ReturnType.GetMethod(nameof(Task.GetAwaiter)) != null;
                    result = isAwaitable ? InvokeAsynchronous(targetMethod, args) : InvokeSynchronous(targetMethod, args);
                    complete = true;
                }
                catch (TargetInvocationException ex)
                {
                    exception = ex.InnerException;
                }
                catch (AggregateException ex)
                {
                    exception = ex.GetBaseException();
                }
                catch (Exception ex)
                {
                    throw new RetryException($"Unexpected exception type '{ex.GetType().FullName}'. See inner exception for details.", ex)
                    {
                        Attempts = currentAttempt,
                        Duration = stopwatch.Elapsed,
                        Exceptions = exceptions
                    };
                }

                // test the exception for can retry, exceeded max retry, and timeout
                if (exception != null)
                {
                    if (exceptions == null)
                    {
                        exceptions = new List<Exception>();
                    }

                    var retryException = WithRetry.TestException(_options, _retryExceptionTester, _exceptionWhiteList, _exceptionBlackList, exception, currentAttempt++, stopwatch.Elapsed, exceptions);
                    if (retryException != null)
                    {
                        throw retryException;
                    }
                }

                //reset exception for next attempt
                exception = null;

                // wait a bit before trying again
                Thread.Sleep(waitTime);

                // ratcheting up - allows wait times up to ~10 seconds per try
                waitTime = TimeSpan.FromMilliseconds(waitTime.TotalMilliseconds <= 110 ? waitTime.TotalMilliseconds * 1.25 : waitTime.TotalMilliseconds);
            }
            return result;
        }

        private object InvokeSynchronous(MethodInfo targetMethod, object[] args)
        {
            return targetMethod.Invoke(_instance, args);
        }

        private object InvokeAsynchronous(MethodInfo targetMethod, object[] args)
        {
            var result = targetMethod.Invoke(_instance, args);
            var task = (Task)result;
            var continuation = task.ContinueWith(t =>
            {
                if (t.Status == TaskStatus.Faulted)
                {
                    throw t.Exception;
                }
            });
            continuation.Wait();
            return result;
        }
    }
}
