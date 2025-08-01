namespace Models;

public class MessageWrapper<T>
{
    public string Message { get; set; }
    public bool Success { get; set; }
    public T? Data { get; set; }

    public MessageWrapper(string message, bool success, T? data)
    {
        Message = message;
        Success = success;
        Data = data;
    }
}