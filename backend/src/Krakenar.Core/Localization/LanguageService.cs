using Krakenar.Contracts.Localization;
using Krakenar.Contracts.Search;
using Krakenar.Core.Localization.Commands;
using Krakenar.Core.Localization.Queries;
using Logitar.CQRS;
using LanguageDto = Krakenar.Contracts.Localization.Language;

namespace Krakenar.Core.Localization;

public class LanguageService : ILanguageService
{
  protected virtual ICommandBus CommandBus { get; }
  protected virtual IQueryBus QueryBus { get; }

  public LanguageService(ICommandBus commandBus, IQueryBus queryBus)
  {
    CommandBus = commandBus;
    QueryBus = queryBus;
  }

  public virtual async Task<CreateOrReplaceLanguageResult> CreateOrReplaceAsync(CreateOrReplaceLanguagePayload payload, Guid? id, long? version, CancellationToken cancellationToken)
  {
    CreateOrReplaceLanguage command = new(id, payload, version);
    return await CommandBus.ExecuteAsync(command, cancellationToken);
  }

  public virtual async Task<LanguageDto?> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    DeleteLanguage command = new(id);
    return await CommandBus.ExecuteAsync(command, cancellationToken);
  }

  public virtual async Task<LanguageDto?> ReadAsync(Guid? id, string? locale, bool isDefault, CancellationToken cancellationToken)
  {
    ReadLanguage query = new(id, locale, isDefault);
    return await QueryBus.ExecuteAsync(query, cancellationToken);
  }

  public virtual async Task<SearchResults<LanguageDto>> SearchAsync(SearchLanguagesPayload payload, CancellationToken cancellationToken)
  {
    SearchLanguages query = new(payload);
    return await QueryBus.ExecuteAsync(query, cancellationToken);
  }

  public virtual async Task<LanguageDto?> SetDefaultAsync(Guid id, CancellationToken cancellationToken)
  {
    SetDefaultLanguage command = new(id);
    return await CommandBus.ExecuteAsync(command, cancellationToken);
  }

  public virtual async Task<LanguageDto?> UpdateAsync(Guid id, UpdateLanguagePayload payload, CancellationToken cancellationToken)
  {
    UpdateLanguage command = new(id, payload);
    return await CommandBus.ExecuteAsync(command, cancellationToken);
  }
}
