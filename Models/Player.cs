using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models;

[Table("Player")]
public class Player
{
    [Key]
    [Required]
    public int Id { get; set; }
    [MaxLength(20)]
    public required string Name { get; set; }
    public int Age { get; set; }

    // Foreign key for Account
    [ForeignKey(nameof(Account))]
    [Required]
    public int AccountId { get; set; }

    public Account? Account { get; set; }
}
