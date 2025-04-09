using Krakenar.Contracts.Realms;

namespace Krakenar.Contracts.Localization;

public class Language : Aggregate
{
  public Realm? Realm { get; set; }

  public bool IsDefault { get; set; }
  public Locale Locale { get; set; } = new();

  public override string ToString() => $"{Locale} | {base.ToString()}";
}
