using System;

namespace ISynergy.Framework.Windows.Common
{
    public class Result
    {
        public bool IsOk { get; set; }
        public string Message { get; set; }
        public string Description { get; set; }
        public Exception Exception { get; set; }

        public static Result Ok()
        {
            return Ok(null, null);
        }
        public static Result Ok(string message = null, string description = null)
        {
            return new Result { IsOk = true, Message = message, Description = description };
        }

        public static Result Error(string message, string description = null)
        {
            return new Result { IsOk = false, Message = message, Description = description };
        }
        public static Result Error(Exception ex)
        {
            return new Result { IsOk = false, Message = ex.Message, Description = ex.ToString(), Exception = ex };
        }

        public override string ToString()
        {
            return $"{Message}\r\n{Description}";
        }
    }

    public class Result<T> : Result
    {
        public T Value { get; private set; }

        public static Result<T> Ok(T result = default)
        {
            return Ok(null, null, result);
        }
        public static Result<T> Ok(string message = null, string description = null, T result = default)
        {
            return new Result<T> { IsOk = true, Message = message, Description = description, Value = result };
        }

        public static Result<T> Error(string message = null, string description = null, T result = default)
        {
            return new Result<T> { IsOk = false, Message = message, Description = description, Value = result };
        }
        public static Result<T> Error(Exception ex, T result = default)
        {
            return new Result<T> { IsOk = false, Message = ex.Message, Description = ex.ToString(), Exception = ex, Value = result };
        }
    }
}
