namespace Models.Util;

public abstract record Dto
{
    public string? Message { get; set; }
    public List<ErrorMessage>? Errors { get; set; }
    public bool Success { get; set; }
}