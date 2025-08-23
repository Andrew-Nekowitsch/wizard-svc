using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Data;

[Table("RefreshToken")]
public class RefreshToken
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [StringLength(100)]
    public required string UserId { get; set; }

    [Required]
    [StringLength(512)]
    public required string Token { get; set; }

    [Required]
    public DateTime Expires { get; set; }

    [Required]
    public DateTime Created { get; set; } = DateTime.UtcNow;

    public DateTime? Revoked { get; set; }
    
    public bool IsActive => Revoked == null && !IsExpired;
    public bool IsExpired => DateTime.UtcNow >= Expires;
}