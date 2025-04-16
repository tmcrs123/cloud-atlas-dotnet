namespace Cloud_Atlas_Dotnet.Domain.Patterns
{
    public class Result
    {
        public bool IsSuccess { get; set; }
        public string Error { get; set; }
        public bool isFailure => !IsSuccess;

        public Result(bool isSuccess, string error)
        {
            if (isFailure && string.IsNullOrEmpty(Error))
            {
                throw new InvalidOperationException("Failure must have an error");
            }

            if (isSuccess && !string.IsNullOrEmpty(Error))
            {
                throw new InvalidOperationException("Success cannot have an error");
            }

            IsSuccess = isSuccess;
            Error = error;
        }

        public static Result Success()
        {
            return new Result(true, string.Empty);
        }

        public static Result Failure(string error)
        {
            return new Result(false, error);
        }
    }

    public class Result<T> : Result
    {
        public T Value { get; set; }

        public Result(T value, bool isSuccess, string error) : base(isSuccess, error)
        {
            Value = value;
        }

        public static Result Success(T value)
        {
            return new Result<T>(value, true, string.Empty);
        }

        public static Result Failure(T value, string error)
        {
            return new Result<T>(default(T), false, error);
        }
    }
}
