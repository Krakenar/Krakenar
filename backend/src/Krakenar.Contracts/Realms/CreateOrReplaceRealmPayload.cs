using Krakenar.Contracts.Settings;

namespace Krakenar.Contracts.Realms;

public record CreateOrReplaceRealmPayload
{
  public string UniqueSlug { get; set; }
  public string? DisplayName { get; set; }
  public string? Description { get; set; }

  public string? Url { get; set; }

  public UniqueNameSettings UniqueNameSettings { get; set; } = new();
  public PasswordSettings PasswordSettings { get; set; } = new();
  public bool RequireUniqueEmail { get; set; }
  public bool RequireConfirmedAccount { get; set; }

  public List<CustomAttribute> CustomAttributes { get; set; } = [];

  public CreateOrReplaceRealmPayload() : this(string.Empty)
  {
  }

  public CreateOrReplaceRealmPayload(string uniqueSlug)
  {
    UniqueSlug = uniqueSlug;
  }
}
