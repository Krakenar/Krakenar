using Krakenar.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;

namespace Krakenar.EntityFrameworkCore.Relational;

public class KrakenarContext : DbContext // TODO(fpion): dependency injection
{
  protected KrakenarContext(DbContextOptions<KrakenarContext> options) : base(options)
  {
  }

  public virtual DbSet<Actor> Actors => Set<Actor>();
  public virtual DbSet<Configuration> Configuration => Set<Configuration>();
  public virtual DbSet<Realm> Realms => Set<Realm>();
  public virtual DbSet<Role> Roles => Set<Role>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
  }
}
