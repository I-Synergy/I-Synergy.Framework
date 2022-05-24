using ISynergy.Framework.Core.Abstractions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ISynergy.Framework.Core.Models
{
    public class Result : IResult
    {
        public Result() { }

        public List<string> Messages { get; set; } = new List<string>();

        public bool Succeeded { get; set; }

        public static IResult Fail() => new Result { Succeeded = false };
    
        public static IResult Fail(string message) => new Result { Succeeded = false, Messages = new List<string> { message } };

        public static IResult Fail(List<string> messages) => new Result { Succeeded = false, Messages = messages };

        public static Task<IResult> FailAsync() => Task.FromResult(Fail());

        public static Task<IResult> FailAsync(string message) => Task.FromResult(Fail(message));

        public static Task<IResult> FailAsync(List<string> messages) => Task.FromResult(Fail(messages));

        public static IResult Success() => new Result { Succeeded = true };

        public static IResult Success(string message) => new Result { Succeeded = true, Messages = new List<string> { message } };

        public static Task<IResult> SuccessAsync() => Task.FromResult(Success());

        public static Task<IResult> SuccessAsync(string message) => Task.FromResult(Success(message));
    }
}
