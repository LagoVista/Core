using System;
using System.Collections.Generic;

namespace LagoVista.Core.Validation
{
    public class InvokeResult<T> : ValidationResult
    {
        public T Result { get; set; }

        public static InvokeResult<T> Create(T result)
        {
            return new InvokeResult<T>() { Result = result } ;
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

        public static InvokeResult<T> FromError(String err)
        {
            var result = new InvokeResult<T>();
            result.Errors.Add(new ErrorMessage(err));
            return result;
        }

        public static InvokeResult<T> FromException(String tag, Exception ex)
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
                result.Errors.Add(new ErrorMessage()
                {
                    ErrorCode = "EXC9997",
                    Message = ex.InnerException.Message,
                    Details = ex.InnerException.StackTrace
                });
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
            foreach(var err in Errors)
            {
                if (String.IsNullOrEmpty(err.ErrorCode))
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
    }


    public class InvokeResult : ValidationResult
    {
        public static InvokeResult Success
        {
            get { return new InvokeResult(); }
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

        public static InvokeResult FromException(String tag, Exception ex)
        {
            var result = new InvokeResult();
            result.Errors.Add(new ErrorMessage()
            {
                ErrorCode = "EXC9998",
                Message = ex.Message,
                Details = ex.StackTrace
            });

            if (ex.InnerException != null)
            {
                result.Errors.Add(new ErrorMessage()
                {
                    ErrorCode = "EXC9997",
                    Message = ex.InnerException.Message,
                    Details = ex.InnerException.StackTrace
                });
            }

            return result;
        }

        public KeyValuePair<string, string>[] ErrorsToKVPArray()
        {
            var idx = 1;
            var kvps = new List<KeyValuePair<string, string>>();
            foreach (var err in Errors)
            {
                if (String.IsNullOrEmpty(err.ErrorCode))
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
    }
}
