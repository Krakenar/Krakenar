using FluentValidation;
using Krakenar.Contracts.Dictionaries;
using Krakenar.Core.Localization;

namespace Krakenar.Core.Messages;

public record Dictionaries
{
  private readonly Dictionary<string, string> _entries = [];

  public Dictionaries()
  {
  }

  public Dictionaries(IReadOnlyDictionary<Locale, Dictionary> dictionaries, Locale defaultLocale, Locale? targetLocale = null)
  {
    if (dictionaries.TryGetValue(defaultLocale, out Dictionary? defaultDictionary))
    {
      Apply(defaultDictionary);
    }

    if (targetLocale is not null)
    {
      try
      {
        Locale fallbackLocale = new(targetLocale.Culture.Parent);
        if (dictionaries.TryGetValue(fallbackLocale, out Dictionary? fallbackDictionary))
        {
          Apply(fallbackDictionary);
        }
      }
      catch (ValidationException)
      {
      }

      if (dictionaries.TryGetValue(targetLocale, out Dictionary? targetDictionary))
      {
        Apply(targetDictionary);
      }
    }
  }

  public void Translate(Identifier key) => Translate(key.Value);
  public string Translate(string key) => _entries.TryGetValue(key, out string? value) ? value : key;

  private void Apply(Dictionary dictionary)
  {
    foreach (DictionaryEntry entry in dictionary.Entries)
    {
      _entries[entry.Key] = entry.Value;
    }
  }
}
