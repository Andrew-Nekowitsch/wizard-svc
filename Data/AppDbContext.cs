using Microsoft.EntityFrameworkCore;
using Models.Data;

namespace Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Account> Accounts { get; set; }
    public DbSet<Player> Players { get; set; }
    public DbSet<Spell> Spells { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
}