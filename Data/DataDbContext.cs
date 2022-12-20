using Microsoft.EntityFrameworkCore;
using Etape1.Models;
namespace Etape1.Data;

public class DataDbContext : DbContext
{

    public DataDbContext(DbContextOptions options ): base(options)
    {

    }
    public DbSet<Users> Users { get; set; }
    public DbSet<Articles> Articles { get; set; }
}
