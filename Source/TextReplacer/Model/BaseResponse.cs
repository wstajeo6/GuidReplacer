using System;

namespace TextReplacer.Model
{
    public abstract class BaseResponse
    {
        public BaseResponse(string message = null, Exception exception = null)
        {
            Message = message;
            Exception = exception;
        }

        public string Message { get; }
        public Exception Exception { get; }
    }

    public abstract class BaseGenericResponse<T> : BaseResponse
    {
        public BaseGenericResponse(T result, string message = null, Exception ex = null) : base(message, ex)
        {
            Result = result;
        }

        public T Result { get; }
    }
}
