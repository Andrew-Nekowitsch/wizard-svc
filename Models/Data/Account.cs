using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Models.Responses;

namespace Models.Data;

[Table("Account")]
public class Account
{
    [Key]
    [Required]
    public int Id { get; set; }
    [MaxLength(254)]
    [Required]
    public required string Email { get; set; }
    [MaxLength(20)]
    [Required]
    public required string UserName { get; set; }
    [MaxLength(20)]
    [Required]
    public required string DisplayName { get; set; }
    [Required]
    public required string PasswordHash { get; set; }
    [Required]
    public ICollection<Player> Players { get; set; } = new List<Player>();
}

public static class AccountExtensions
{
    public static GetAccountResponse ToGetAccountResponse(this Account account)
    {
        return new GetAccountResponse
        {
            Id = account.Id,
            Username = account.UserName
        };
    }
}