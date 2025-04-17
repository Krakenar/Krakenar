using Krakenar.Contracts;
using LanguageDto = Krakenar.Contracts.Localization.Language;

namespace Krakenar.Core.Localization.Queries;

public record ReadLanguage(Guid? Id, string? Locale, bool IsDefault) : IQuery<LanguageDto?>;

/// <exception cref="TooManyResultsException{T}"></exception>
public class ReadLanguageHandler : IQueryHandler<ReadLanguage, LanguageDto?>
{
  protected virtual ILanguageQuerier LanguageQuerier { get; }

  public ReadLanguageHandler(ILanguageQuerier languageQuerier)
  {
    LanguageQuerier = languageQuerier;
  }

  public virtual async Task<LanguageDto?> HandleAsync(ReadLanguage query, CancellationToken cancellationToken)
  {
    Dictionary<Guid, LanguageDto> languages = new(capacity: 3);

    if (query.Id.HasValue)
    {
      LanguageDto? language = await LanguageQuerier.ReadAsync(query.Id.Value, cancellationToken);
      if (language is not null)
      {
        languages[language.Id] = language;
      }
    }

    if (!string.IsNullOrWhiteSpace(query.Locale))
    {
      LanguageDto? language = await LanguageQuerier.ReadAsync(query.Locale, cancellationToken);
      if (language is not null)
      {
        languages[language.Id] = language;
      }
    }

    if (query.IsDefault)
    {
      LanguageDto language = await LanguageQuerier.ReadDefaultAsync(cancellationToken);
      languages[language.Id] = language;
    }

    if (languages.Count > 1)
    {
      throw TooManyResultsException<LanguageDto>.ExpectedSingle(languages.Count);
    }

    return languages.SingleOrDefault().Value;
  }
}
