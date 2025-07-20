namespace HCM.Shared.Results;

public class ResultWithoutValue
{
    private readonly CustomError error;
    
    protected ResultWithoutValue(bool isSuccess, CustomError error)
    {
        if ((isSuccess && error != CustomError.None) ||
            (!isSuccess && error == CustomError.None))
            throw new ArgumentException("Invalid error", nameof(error));
        
        IsSuccess = isSuccess;
        this.error = error;
    }
    
    public CustomError Error => !IsSuccess ? error : throw new InvalidOperationException("Result is successful");
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    
    public static ResultWithoutValue Success() => new(true, CustomError.None);
    public static ResultWithoutValue NotFound(string message) => new(false, new CustomError(404, message));
    public static ResultWithoutValue Invalid(string message) => new(false, new CustomError(400, message));
    public static ResultWithoutValue Failure(string message) => new(false, new CustomError(500, message));
    public static ResultWithoutValue CreateError(CustomError customError) => new(false, customError);
}
