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

public static class MessageWrapperExtensions
{
    public static List<ErrorMessage> Validate<T>(this MessageWrapper<T>? wrapper)
    {
        var errors = new List<ErrorMessage>();

        if (wrapper == null)
        {
            errors.Add(new ErrorMessage("wrapper", "MessageWrapper is null."));
            return errors;
        }

        if (!wrapper.Success)
        {
            errors.AddRange(wrapper.Error);
        }

        if (string.IsNullOrWhiteSpace(wrapper.Message))
        {
            errors.Add(new ErrorMessage("message", "Message is null or empty."));
        }

        if (wrapper.Data == null)
        {
            errors.Add(new ErrorMessage("data", "Data is null."));
        }

        return errors;
    }
}

public static class ErrorMessageExtensions
{
    public static bool ContainsErrors(this IEnumerable<ErrorMessage>? errors, string[] errorTypes)
    {
        if (errors == null || !errors.Any())
        {
            return false;
        }

        foreach (var error in errors)
        {
            if (errorTypes.Contains(error.Error))
            {
                return true;
            }
        }

        return false;
    }
    
    public static bool ContainsErrors(this IEnumerable<ErrorMessage>? errors)
    {
        if (errors == null)
        {
            return false;
        }

        foreach (var error in errors)
        {
            if (error.Error == "exception" || error.Error == "message" || error.Error == "data")    
            {
                return true;
            }   
        }

        return false;
    }
}