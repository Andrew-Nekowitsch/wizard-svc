namespace Models.Responses;

public record LoginResponse()
{
    public required TokenResponse Tokens { get; set; }
    public required MessageWrapper<GetAccountResponse> UserWrapper { get; set; }
}