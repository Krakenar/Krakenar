using Krakenar.Contracts.Localization;
using Krakenar.Contracts.Search;
using Krakenar.Core.Localization.Commands;
using Krakenar.Core.Localization.Queries;
using LanguageDto = Krakenar.Contracts.Localization.Language;

namespace Krakenar.Core.Localization;

public class LanguageService : ILanguageService
{
  protected virtual ICommandHandler<CreateOrReplaceLanguage, CreateOrReplaceLanguageResult> CreateOrReplaceLanguage { get; }
  protected virtual ICommandHandler<DeleteLanguage, LanguageDto?> DeleteLanguage { get; }
  protected virtual IQueryHandler<ReadLanguage, LanguageDto?> ReadLanguage { get; }
  protected virtual IQueryHandler<SearchLanguages, SearchResults<LanguageDto>> SearchLanguages { get; }
  protected virtual ICommandHandler<SetDefaultLanguage, LanguageDto?> SetDefaultLanguage { get; }
  protected virtual ICommandHandler<UpdateLanguage, LanguageDto?> UpdateLanguage { get; }

  public LanguageService(
    ICommandHandler<CreateOrReplaceLanguage, CreateOrReplaceLanguageResult> createOrReplaceLanguage,
    ICommandHandler<DeleteLanguage, LanguageDto?> deleteLanguage,
    IQueryHandler<ReadLanguage, LanguageDto?> readLanguage,
    IQueryHandler<SearchLanguages, SearchResults<LanguageDto>> searchLanguages,
    ICommandHandler<SetDefaultLanguage, LanguageDto?> setDefaultLanguage,
    ICommandHandler<UpdateLanguage, LanguageDto?> updateLanguage)
  {
    CreateOrReplaceLanguage = createOrReplaceLanguage;
    DeleteLanguage = deleteLanguage;
    ReadLanguage = readLanguage;
    SearchLanguages = searchLanguages;
    SetDefaultLanguage = setDefaultLanguage;
    UpdateLanguage = updateLanguage;
  }

  public virtual async Task<CreateOrReplaceLanguageResult> CreateOrReplaceAsync(CreateOrReplaceLanguagePayload payload, Guid? id, long? version, CancellationToken cancellationToken)
  {
    CreateOrReplaceLanguage command = new(id, payload, version);
    return await CreateOrReplaceLanguage.HandleAsync(command, cancellationToken);
  }

  public virtual async Task<LanguageDto?> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    DeleteLanguage command = new(id);
    return await DeleteLanguage.HandleAsync(command, cancellationToken);
  }

  public virtual async Task<LanguageDto?> ReadAsync(Guid? id, string? locale, bool isDefault, CancellationToken cancellationToken)
  {
    ReadLanguage query = new(id, locale, isDefault);
    return await ReadLanguage.HandleAsync(query, cancellationToken);
  }

  public virtual async Task<SearchResults<LanguageDto>> SearchAsync(SearchLanguagesPayload payload, CancellationToken cancellationToken)
  {
    SearchLanguages query = new(payload);
    return await SearchLanguages.HandleAsync(query, cancellationToken);
  }

  public virtual async Task<LanguageDto?> SetDefaultAsync(Guid id, CancellationToken cancellationToken)
  {
    SetDefaultLanguage command = new(id);
    return await SetDefaultLanguage.HandleAsync(command, cancellationToken);
  }

  public virtual async Task<LanguageDto?> UpdateAsync(Guid id, UpdateLanguagePayload payload, CancellationToken cancellationToken)
  {
    UpdateLanguage command = new(id, payload);
    return await UpdateLanguage.HandleAsync(command, cancellationToken);
  }
}
