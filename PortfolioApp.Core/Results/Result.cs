namespace PortfolioApp.Core.Results;

public class Result : IResult
{
    public bool Success { get; }
    public string Message { get; }

    protected Result(bool success, string message)
    {
        Success = success;
        Message = message;
    }

    public static IResult Ok(string message = "İşlem başarılı.") => new Result(true, message);
    public static IResult Fail(string message = "İşlem başarısız.") => new Result(false, message);
}
