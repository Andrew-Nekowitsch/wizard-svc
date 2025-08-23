using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Data;

[Table("PlayerSpell")]
public class PlayerSpell
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public int PlayerId { get; set; }

    [ForeignKey("PlayerId")]
    public Player Player { get; set; } = null!;
    
    [Required]
    public int SpellId { get; set; }

    [ForeignKey("SpellId")]
    public Spell Spell { get; set; } = null!;
}