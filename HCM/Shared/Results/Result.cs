namespace HCM.Shared.Results;

public class Result<T> : ResultWithoutValue
{
    private T? value;
    
    private Result(bool isSuccess, T? value, CustomError error) : base(isSuccess, error) => this.value = value;
    
    public T Value
    {
        get => IsSuccess ? value! : throw new InvalidOperationException("Result is not successful");
        private set => this.value = value;
    }
    
    public static implicit operator Result<T>(T value) => Success(value);
    public Result<TA> AsError<TA>() => Result<TA>.CustomError(Error);
    
#pragma warning disable CA1000
    public static Result<T> Success(T result) => new(true, result, Results.CustomError.None);
    public static new Result<T> NotFound(string message) => new(false, default, new CustomError(404, message));
    public static new Result<T> Invalid(string message) => new(false, default, new CustomError(400, message));
    public static new Result<T> Failure(string message) => new(false, default, new CustomError(500, message)); 
    public static Result<T> CustomError(CustomError customError) => new(false, default, customError);
    
#pragma warning restore CA1000
}
