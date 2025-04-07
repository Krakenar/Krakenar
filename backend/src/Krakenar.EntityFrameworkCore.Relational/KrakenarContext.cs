using Krakenar.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;

namespace Krakenar.EntityFrameworkCore.Relational;

public class KrakenarContext : DbContext // TODO(fpion): dependency injection
{
  protected KrakenarContext(DbContextOptions<KrakenarContext> options) : base(options)
  {
  }

  public DbSet<Actor> Actors => Set<Actor>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
  }
}
