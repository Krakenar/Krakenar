using Krakenar.Contracts.Settings;

namespace Krakenar.Contracts.Realms;

public class Realm : Aggregate
{
  public string UniqueSlug { get; set; }
  public string? DisplayName { get; set; }
  public string? Description { get; set; }

  [JsonIgnore]
  public string? Secret { get; set; }

  public string? Url { get; set; }

  public UniqueNameSettings UniqueNameSettings { get; set; } = new();
  public PasswordSettings PasswordSettings { get; set; } = new();
  public bool RequireUniqueEmail { get; set; }
  public bool RequireConfirmedAccount { get; set; }

  public List<CustomAttribute> CustomAttributes { get; set; } = [];

  public Realm() : this(string.Empty)
  {
  }

  public Realm(string uniqueSlug)
  {
    UniqueSlug = uniqueSlug;
  }

  public override string ToString() => $"{DisplayName ?? UniqueSlug} | {base.ToString()}";
}
