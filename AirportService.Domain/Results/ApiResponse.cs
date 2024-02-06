public class ApiResponse
{
    public int StatusCode { get; set; }
    public string Message { get; set; }
    public object Data { get; set; }
    public IEnumerable<string> Errors { get; set; }
}
public class ApiResponse<T>
{
    public int StatusCode { get; set; }
    public string Message { get; set; }
    public T Data { get; set; }
}