using Microsoft.EntityFrameworkCore;
using TrafficGuard.Domain.Entities;

namespace TrafficGuard.Infrastructure.Data;

public class TrafficGuardDbContext : DbContext
{
    public DbSet<TrafficViolation> Violations { get; set; }

    public TrafficGuardDbContext(DbContextOptions<TrafficGuardDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TrafficGuardDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}