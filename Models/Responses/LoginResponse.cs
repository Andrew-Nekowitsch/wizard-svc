namespace Models.Responses;

public record LoginResponse()
{
    public required string AccessToken { get; set; }
    public required MessageWrapper<GetAccountResponse> UserWrapper { get; set; }
}