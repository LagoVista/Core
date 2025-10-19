// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 0b9d9c5025514363220fcfe10b12e19483da7049cd810ca96373e9847fa062a5
// IndexVersion: 0
// --- END CODE INDEX META ---
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace LagoVista.Core.Retry
{
    public static class WithRetry
    {
        private static RetryOptions _defaultRetryOptions = null;
        public static void SetDefaultOptions(RetryOptions options)
        {
            _defaultRetryOptions = options;
        }

        #region void Invoke(Action action)
        public static void Invoke(Action action)
        {
            Invoke(_defaultRetryOptions, action);
        }

        public static void Invoke(RetryOptions options, Action action, IRetryExceptionTester retryExceptionTester = null, IEnumerable<Type> exceptionWhiteList = null, IEnumerable<Type> exceptionBlackList = null)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            List<Exception> exceptions = null;

            var waitTime = options.InitialInterval;
            var currentAttempt = 1;
            Exception exception = null;

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var complete = false;
            while (!complete)
            {
                try
                {
                    action();
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
                    exception = ex;
                }

                // test the exception for can retry, exceeded max retry, and timeout
                if (exception != null)
                {
                    if (exceptions == null)
                    {
                        exceptions = new List<Exception>();
                    }

                    var retryException = TestException(options, retryExceptionTester, exceptionWhiteList, exceptionBlackList, exception, currentAttempt++, stopwatch.Elapsed, exceptions);
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
        }
        #endregion

        #region T Invoke<T>(Func<T> function)
        public static T Invoke<T>(Func<T> function)
        {
            return Invoke(_defaultRetryOptions, function);
        }

        public static T Invoke<T>(RetryOptions options, Func<T> function, IRetryExceptionTester retryExceptionTester = null, IEnumerable<Type> exceptionWhiteList = null, IEnumerable<Type> exceptionBlackList = null)
        {
            if (function == null)
            {
                throw new ArgumentNullException(nameof(function));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            var isAwaitable = function.Method.ReturnType.GetMethod(nameof(Task.GetAwaiter)) != null;
            if (isAwaitable)
            {
                throw new NotSupportedException("Use InvokeAsync<T> instead.");
            }

            var result = default(T);

            List<Exception> exceptions = null;

            var waitTime = options.InitialInterval;
            var currentAttempt = 1;
            Exception exception = null;

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var complete = false;
            while (!complete)
            {
                try
                {
                    result = function();
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
                    exception = ex;
                }

                // test the exception for can retry, exceeded max retry, and timeout
                if (exception != null)
                {
                    if (exceptions == null)
                    {
                        exceptions = new List<Exception>();
                    }

                    var retryException = TestException(options, retryExceptionTester, exceptionWhiteList, exceptionBlackList, exception, currentAttempt++, stopwatch.Elapsed, exceptions);
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
        #endregion

        #region Task<T> InvokeAsync<T>(Func<Task<T>> function)
        public static Task<T> InvokeAsync<T>(Func<Task<T>> function)
        {
            return InvokeAsync(_defaultRetryOptions, function);
        }

        public static async Task<T> InvokeAsync<T>(RetryOptions options, Func<Task<T>> function, IRetryExceptionTester retryExceptionTester = null, IEnumerable<Type> exceptionWhiteList = null, IEnumerable<Type> exceptionBlackList = null)
        {
            if (function == null)
            {
                throw new ArgumentNullException(nameof(function));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            var result = default(T);

            List<Exception> exceptions = null;

            var waitTime = options.InitialInterval;
            var currentAttempt = 1;
            Exception exception = null;

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var complete = false;
            while (!complete)
            {
                try
                {
                    result = await function();
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
                    exception = ex;
                }

                // test the exception for can retry, exceeded max retry, and timeout
                if (exception != null)
                {
                    if (exceptions == null)
                    {
                        exceptions = new List<Exception>();
                    }

                    var retryException = TestException(options, retryExceptionTester, exceptionWhiteList, exceptionBlackList, exception, currentAttempt++, stopwatch.Elapsed, exceptions);
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
        #endregion

        internal static RetryException TestException(RetryOptions options, IRetryExceptionTester retryExceptionTester, IEnumerable<Type> exceptionWhiteList, IEnumerable<Type> exceptionBlackList, Exception exception, int currentAttempt, TimeSpan elapsed, List<Exception> exceptions)
        {
            exceptions.Add(exception);

            //simplified WithRetry class, added better async support, added wait interval to RetryOptions, added support for exception white lists and black lists
            var type = exception.GetType();

            if (exceptionWhiteList != null && !exceptionWhiteList.Contains(type))
            {
                return new RetryNotAllowedException(RetryNotAllowedReason.WhiteList, "Exception does not qualify for retry. See inner exception for details.", exception)
                {
                    Attempts = currentAttempt,
                    Duration = elapsed,
                    Exceptions = exceptions
                };
            }

            if (exceptionBlackList != null && exceptionBlackList.Contains(type))
            {
                return new RetryNotAllowedException(RetryNotAllowedReason.BlackList, "Exception does not qualify for retry. See inner exception for details.", exception)
                {
                    Attempts = currentAttempt,
                    Duration = elapsed,
                    Exceptions = exceptions
                };
            }

            if (retryExceptionTester != null && exception != null && !retryExceptionTester.CanRetry(exception))
            {
                return new RetryNotAllowedException(RetryNotAllowedReason.RetryTester, "Exception does not qualify for retry. See inner exception for details.", exception)
                {
                    Attempts = currentAttempt,
                    Duration = elapsed,
                    Exceptions = exceptions
                };
            }

            if (currentAttempt >= options.MaxAttempts)
            {
                return new ExceededMaxAttemptsException($"Exceeded maximum attempts: {options.MaxAttempts}. See Exceptions property for details.")
                {
                    Attempts = currentAttempt,
                    Duration = elapsed,
                    Exceptions = exceptions
                };
            }

            if (elapsed >= options.Timeout)
            {
                return new ExceededMaxWaitTimeException($"Exceeded maximum wait time: {options.Timeout} seconds. See Exceptions property for details.")
                {
                    Attempts = currentAttempt,
                    Duration = elapsed,
                    Exceptions = exceptions
                };
            }

            return null;
        }
    }
}

