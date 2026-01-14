using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Renligou.Core.Shared.Ddd
{
    public class Result
    {
        private Result(bool success, Error? error)
        {
            Success = success;
            Error = error;
        }

        public bool Success { get; }
        public Error? Error { get; }

        public static Result Ok() => new(true, null);

        public static Result Fail(string code, string message)
            => new(false, new Error(code, message));

        public static Result Fail(Error error)
            => new(false, error);

        public void EnsureSuccess()
        {
            if (!Success)
                throw new InvalidOperationException(Error?.Message);
        }
    }

    public sealed class Result<T>
    {
        private Result(bool success, T? value, Error? error)
        {
            Success = success;
            Value = value;
            Error = error;
        }

        public bool Success { get; }
        public T? Value { get; }
        public Error? Error { get; }

        public static Result<T> Ok(T value)
            => new(true, value, null);

        public static Result<T> Fail(string code, string message)
            => new(false, default, new Error(code, message));

        public static Result<T> Fail(Error error)
            => new(false, default, error);

        public T EnsureSuccess()
        {
            if (!Success)
                throw new InvalidOperationException(Error?.Message);

            return Value!;
        }
    }
}
