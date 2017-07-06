using System;

namespace LagoVista.Core.Validation
{
    public class InvokeResult<T> : ValidationResult
    {
        public T Result { get; set; }

        public static InvokeResult<T> Create(T result)
        {
            return new InvokeResult<T>() { Result = result } ;
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
    }
}
