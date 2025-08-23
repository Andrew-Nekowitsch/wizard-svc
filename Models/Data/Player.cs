using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Data;

[Table("Player")]
public class Player
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [ForeignKey(nameof(Account))]
    public int AccountId { get; set; }

    [Required]
    [MaxLength(30)]
    public string Name { get; set; } = string.Empty;

    // Navigation properties
    public ICollection<Spell> Spells { get; set; } = new List<Spell>();
    public Account Account { get; set; } = null!;
}