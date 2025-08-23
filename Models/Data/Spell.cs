using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Data;

[Table("Spells")]
public class Spell
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public int PlayerId { get; set; }

    [ForeignKey("PlayerId")]
    public Player Player { get; set; } = null!;

    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(255)]
    public string Description { get; set; } = string.Empty;

    [Required]
    public int ManaCost { get; set; }

    [Required]
    public int Power { get; set; }

    [Required]
    public int CooldownSeconds { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}