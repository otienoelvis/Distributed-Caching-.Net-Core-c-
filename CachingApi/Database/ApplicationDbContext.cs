using System.Data;
using System.Reflection;
using CachingApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CachingApi.Database;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
    
    public IDbConnection Connection => Database.GetDbConnection();

    public DbSet<Driver> Driver { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}