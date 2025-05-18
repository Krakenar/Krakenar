using Krakenar.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;

namespace Krakenar.EntityFrameworkCore.Relational;

public sealed class KrakenarContext : DbContext
{
  public KrakenarContext(DbContextOptions<KrakenarContext> options) : base(options)
  {
  }

  public DbSet<Actor> Actors => Set<Actor>();
  public DbSet<ApiKey> ApiKeys => Set<ApiKey>();
  public DbSet<ApiKeyRole> ApiKeyRoles => Set<ApiKeyRole>();
  public DbSet<BlacklistedToken> TokenBlacklist => Set<BlacklistedToken>();
  public DbSet<Configuration> Configuration => Set<Configuration>();
  public DbSet<Content> Contents => Set<Content>();
  public DbSet<ContentLocale> ContentLocales => Set<ContentLocale>();
  public DbSet<ContentType> ContentTypes => Set<ContentType>();
  public DbSet<CustomAttribute> CustomAttributes => Set<CustomAttribute>();
  public DbSet<Dictionary> Dictionaries => Set<Dictionary>();
  public DbSet<DictionaryEntry> DictionaryEntries => Set<DictionaryEntry>();
  public DbSet<FieldDefinition> FieldDefinitions => Set<FieldDefinition>();
  public DbSet<FieldType> FieldTypes => Set<FieldType>();
  public DbSet<Language> Languages => Set<Language>();
  public DbSet<Message> Messages => Set<Message>();
  public DbSet<OneTimePassword> OneTimePasswords => Set<OneTimePassword>();
  public DbSet<PublishedContent> PublishedContents => Set<PublishedContent>();
  public DbSet<Realm> Realms => Set<Realm>();
  public DbSet<Recipient> Recipients => Set<Recipient>();
  public DbSet<Role> Roles => Set<Role>();
  public DbSet<Sender> Senders => Set<Sender>();
  public DbSet<Session> Sessions => Set<Session>();
  public DbSet<Template> Templates => Set<Template>();
  public DbSet<User> Users => Set<User>();
  public DbSet<UserIdentifier> UserIdentifiers => Set<UserIdentifier>();
  public DbSet<UserRole> UserRoles => Set<UserRole>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
  }
}
