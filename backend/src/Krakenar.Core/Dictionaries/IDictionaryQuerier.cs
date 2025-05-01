using Krakenar.Core.Localization;

namespace Krakenar.Core.Dictionaries;

public interface IDictionaryQuerier
{
  Task<DictionaryId?> FindIdAsync(LanguageId languageId, CancellationToken cancellationToken = default);
}
