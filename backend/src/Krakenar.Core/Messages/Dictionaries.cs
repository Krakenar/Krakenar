using Krakenar.Contracts.Dictionaries;
using Krakenar.Core.Localization;

namespace Krakenar.Core.Messages;

public record Dictionaries
{
  public Dictionaries(IReadOnlyDictionary<Locale, Dictionary> dictionaries, Locale defaultLocale, Locale? targetLocale = null)
  {
    // TODO(fpion): implement
  }

  public void Translate(Identifier key) => Translate(key.Value);
  public string Translate(string key)
  {
    // TODO(fpion): implement
  }
}
