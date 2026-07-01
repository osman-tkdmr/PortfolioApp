namespace PortfolioApp.Core.Results;

public class DataResult<T> : Result, IDataResult<T>
{
    public T? Data { get; }

    protected DataResult(bool success, string message, T? data) : base(success, message)
    {
        Data = data;
    }

    public static IDataResult<T> Ok(T data, string message = "İşlem başarılı.") =>
        new DataResult<T>(true, message, data);

    public static new IDataResult<T> Fail(string message = "İşlem başarısız.") =>
        new DataResult<T>(false, message, default);
}
