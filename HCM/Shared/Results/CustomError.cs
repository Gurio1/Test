namespace HCM.Shared.Results;

public sealed record CustomError(int Code, string Description)
{
    public static readonly CustomError None = new(0, string.Empty);
}
