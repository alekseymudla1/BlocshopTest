using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace BlocshopTest.EF;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            Assembly.GetAssembly(typeof(ApplicationDbContext)));
    }
}
