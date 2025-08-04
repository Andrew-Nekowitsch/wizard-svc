using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models;

[Table("RefreshToken")]
public class RefreshToken
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [StringLength(100)]
    public required string UserId { get; set; }

    [Required]
    [StringLength(512)]
    public required string Token { get; set; }

    [Required]
    public DateTime Expires { get; set; }

    public DateTime? Revoked { get; set; }
}