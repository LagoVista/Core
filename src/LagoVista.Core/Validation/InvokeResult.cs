using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace LagoVista.Core.Validation
{
    public class InvokeResult<T> : ValidationResult
    {
        public T Result { get; set; }

        public string ContentType { get; set; } = "application/json";

        public string RedirectURL { get; set; }

        public static InvokeResult<T> Create(T result)
        {
            return new InvokeResult<T>() { Result = result };
        }

        public static InvokeResult<T> Create(T result, List<ResultTiming> timings)
        {
            return new InvokeResult<T>() { Result = result, Timings = timings };
        }

        public static InvokeResult<T> Create(T result, TimingBuilder bldr)
        {
            return new InvokeResult<T>() { Result = result, Timings = bldr.ResultTimings };
        }


        public static InvokeResult<T> Create(T result, string url)
        {
            return new InvokeResult<T>() { Result = result, RedirectURL = url };
        }


        /// <summary>
        /// Create an empty result that can either be populated at a later time
        /// or used to show success.
        /// </summary>
        /// <returns></returns>
        public static InvokeResult<T> CreateEmpty()
        {
            return new InvokeResult<T>() { };
        }


        public static InvokeResult<T> FromErrors(params ErrorMessage[] errs)
        {
            var result = new InvokeResult<T>();
            foreach (var err in errs)
            {
                result.Errors.Add(err);
            }

            return result;
        }

        public static InvokeResult<T> FromError(string err, string errorCode = "")
        {
            var result = new InvokeResult<T>();
            result.Errors.Add(new ErrorMessage(errorCode, err));
            return result;
        }

        public static InvokeResult<T> FromException(string tag, Exception ex)
        {
            var result = new InvokeResult<T>();
            result.Errors.Add(new ErrorMessage()
            {
                ErrorCode = "EXC9999",
                Message = "Unhandled Excpetion",
                Details = tag
            });

            result.Errors.Add(new ErrorMessage()
            {
                ErrorCode = "EXC9998",
                Message = ex.Message,
                Details = ex.StackTrace
            });

            if (ex.InnerException != null)
            {
                if (ex.InnerException.Message != ex.Message)
                {
                    result.Errors.Add(new ErrorMessage()
                    {
                        ErrorCode = "EXC9997",
                        Message = ex.InnerException.Message,
                        Details = ex.InnerException.StackTrace
                    });
                }
            }

            return result;
        }

        public static InvokeResult<T> FromInvokeResult(InvokeResult originalResult)
        {
            var result = new InvokeResult<T>();
            result.Concat(originalResult);
            return result;
        }

        public KeyValuePair<string, string>[] ErrorsToKVPArray()
        {
            var idx = 1;
            var kvps = new List<KeyValuePair<string, string>>();
            foreach (var err in Errors)
            {
                if (string.IsNullOrEmpty(err.ErrorCode))
                {
                    kvps.Add(new KeyValuePair<string, string>($"Err{idx++}", err.Message));
                }
                else
                {
                    kvps.Add(new KeyValuePair<string, string>($"Err{idx++}", $"{err.ErrorCode} - {err.Message}"));
                }
            }
            return kvps.ToArray();
        }

        public List<ResultTiming> Timings { get; set; } = new List<ResultTiming>();

        public override InvokeResult ToInvokeResult()
        {
            var invokeResult = new InvokeResult();
            invokeResult.Errors.AddRange(this.Errors);
            invokeResult.Warnings.AddRange(this.Warnings);
            invokeResult.Timings.AddRange(this.Timings);

            return invokeResult;            
        }
    }


    public class InvokeResult : ValidationResult
    {
        public string RedirectURL { get; set; }

        public static InvokeResult Success => new InvokeResult();

        public static InvokeResult SuccessRedirect(string url) => new InvokeResult() { RedirectURL = url};


        public static InvokeResult FromError(string err, string errorCode = "", List<ResultTiming> timings = null)
        {
            var result = new InvokeResult();
            if(timings != null)
                result.Timings.AddRange(timings);

            result.Errors.Add(new ErrorMessage(errorCode, err));
            return result;
        }

        public static InvokeResult FromErrors(params ErrorMessage[] errs)
        {
            var result = new InvokeResult();
            foreach (var err in errs)
            {
                result.Errors.Add(err);
            }

            return result;
        }

        public static InvokeResult FromException(string tag, Exception ex, List<ResultTiming> timings = null)
        {
            var result = new InvokeResult();
            result.Errors.Add(new ErrorMessage()
            {
                ErrorCode = "EXC9998",
                Message = ex.Message,
                Details = ex.StackTrace
            });

            if (timings != null)
                result.Timings.AddRange(timings);

            if (ex.InnerException != null)
            {
                if (ex.Message != ex.InnerException.Message)
                {
                    result.Errors.Add(new ErrorMessage()
                    {
                        ErrorCode = "EXC9997",
                        Message = ex.InnerException.Message,
                        Details = ex.InnerException.StackTrace
                    });
                }
            }

            return result;
        }

        public KeyValuePair<string, string>[] ErrorsToKVPArray()
        {
            var idx = 1;
            var kvps = new List<KeyValuePair<string, string>>();
            foreach (var err in Errors)
            {
                if (string.IsNullOrEmpty(err.ErrorCode))
                {
                    kvps.Add(new KeyValuePair<string, string>($"Err{idx++}", err.Message));
                }
                else
                {
                    kvps.Add(new KeyValuePair<string, string>($"Err{idx++}", $"{err.ErrorCode} - {err.Message}"));
                }
            }
            return kvps.ToArray();
        }

        public override string ToString()
        {
            var bldr = new StringBuilder();
            if (Errors.Count == 0)
            {
                bldr.Append("Success");
            }
            else
            {
                bldr.Append("Failed: ");
                foreach(var err in Errors)
                {
                    bldr.Append($" {err};");
                }
            }

            return bldr.ToString();
        }

        public List<ResultTiming> Timings { get; set; } = new List<ResultTiming>();
    }

    public class ResultTiming
    {
        public string Key { get; set; }
        public double Ms { get; set; }
    }

    public class TimingBuilder
    {
        Stopwatch _sw;

        public List<ResultTiming> ResultTimings { get; private set; } = new List<ResultTiming>();

        public static TimingBuilder StartNew()
        {
            return new TimingBuilder()
            {
                _sw = Stopwatch.StartNew()
            };
        }

        public void CompleteAndRestart(string message)
        {
            ResultTimings.Add(new ResultTiming() { Key = message, Ms = _sw.Elapsed.TotalMilliseconds });
            _sw.Restart();
        }
    }
}
