namespace Models;

public class MessageWrapper<T>(string message, List<ErrorMessage> error, bool success, T? data)
{
    public string Message { get; set; } = message;
    public List<ErrorMessage> Error { get; set; } = error;
    public bool Success { get; set; } = success;
    public T? Data { get; set; } = data;
}

public class ErrorMessage(string message, string error)
{
    public string Message { get; set; } = message;
    public string Error { get; set; } = error;
}