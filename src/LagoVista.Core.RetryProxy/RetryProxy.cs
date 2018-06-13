using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace LagoVista.Core.Retry
{
    public class RetryProxy : DispatchProxy
    {
        private object _instance;
        private RetryOptions _options;
        private Func<Exception, bool> _isTransientException = null;
        private IEnumerable<Type> _transientExceptions = null;

        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            object result = null;
            var waitTime = 1.0;
            var currentAttempt = 0;
            var complete = false;
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            while (!complete)
            {
                try
                {
                    result = targetMethod.Invoke(_instance, args);
                    complete = true;
                }
                catch (Exception ex)
                {
                    var exceptionType = ex.GetType();
                    if ((_isTransientException != null && !_isTransientException(ex)) || (_transientExceptions != null && !_transientExceptions.Contains(exceptionType)))
                    {
                        throw new NotTransientException("Exception does not qualify for retry. See inner exception for details.", ex);
                    }

                    if (currentAttempt >= _options.MaxAttempts)
                    {
                        throw new ExceededMaxAttemptsException($"Exceeded maximum attempts: {_options.MaxAttempts}.. See inner exception for details.", ex);
                    }

                    if (stopwatch.ElapsedMilliseconds >= _options.MaxWaitTimeInSeconds * 1000)
                    {
                        throw new ExceededMaxWaitTimeException($"Exceeded maximum wait time: {_options.MaxWaitTimeInSeconds} seconds.. See inner exception for details.", ex);
                    }

                    Thread.Sleep((int)(waitTime * 100));

                    // allow wait times up to ~10 seconds
                    waitTime = waitTime <= 110 ? waitTime * 1.25 : waitTime;
                }
            }
            return result;
        }

        public static TInterface Create<TInterface>(TInterface instance, RetryOptions options, Func<Exception, bool> isTransientException = null) where TInterface : class
        {
            var result = Create<TInterface, RetryProxy>();

            var proxy = (result as RetryProxy);
            proxy._instance = instance ?? throw new ArgumentNullException(nameof(instance));
            proxy._options = options ?? throw new ArgumentNullException(nameof(options));
            proxy._isTransientException = isTransientException;

            return result;
        }

        public static TInterface Create<TInterface>(TInterface instance, RetryOptions options, IEnumerable<Type> transientExceptions) where TInterface : class
        {
            var result = Create<TInterface, RetryProxy>();

            var proxy = (result as RetryProxy);
            proxy._instance = instance ?? throw new ArgumentNullException(nameof(instance));
            proxy._options = options ?? throw new ArgumentNullException(nameof(options));
            proxy._transientExceptions = transientExceptions ?? throw new ArgumentNullException(nameof(transientExceptions));

            return result;
        }
    }
}
