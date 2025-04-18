﻿using Krakenar.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;

namespace Krakenar.EntityFrameworkCore.Relational;

public sealed class KrakenarContext : DbContext
{
  public KrakenarContext(DbContextOptions<KrakenarContext> options) : base(options)
  {
  }

  public DbSet<Actor> Actors => Set<Actor>();
  public DbSet<Configuration> Configuration => Set<Configuration>();
  public DbSet<CustomAttribute> CustomAttributes => Set<CustomAttribute>();
  public DbSet<Language> Languages => Set<Language>();
  public DbSet<Realm> Realms => Set<Realm>();
  public DbSet<Role> Roles => Set<Role>();
  public DbSet<Session> Sessions => Set<Session>();
  public DbSet<User> Users => Set<User>();
  public DbSet<UserIdentifier> UserIdentifiers => Set<UserIdentifier>();
  public DbSet<UserRole> UserRoles => Set<UserRole>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
  }
}
