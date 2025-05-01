using LanguageDto = Krakenar.Contracts.Localization.Language;

namespace Krakenar.Core.Localization.Commands;

public record DeleteLanguage(Guid Id) : ICommand<LanguageDto?>;

public class DeleteLanguageHandler : ICommandHandler<DeleteLanguage, LanguageDto?>
{
  protected virtual IApplicationContext ApplicationContext { get; }
  protected virtual ILanguageQuerier LanguageQuerier { get; }
  protected virtual ILanguageRepository LanguageRepository { get; }

  public DeleteLanguageHandler(IApplicationContext applicationContext, ILanguageQuerier languageQuerier, ILanguageRepository languageRepository)
  {
    ApplicationContext = applicationContext;
    LanguageQuerier = languageQuerier;
    LanguageRepository = languageRepository;
  }

  public virtual async Task<LanguageDto?> HandleAsync(DeleteLanguage command, CancellationToken cancellationToken)
  {
    LanguageId languageId = new(command.Id, ApplicationContext.RealmId);
    Language? language = await LanguageRepository.LoadAsync(languageId, cancellationToken);
    if (language is null)
    {
      return null;
    }
    else if (language.IsDefault)
    {
      throw new CannotDeleteDefaultLanguageException(language);
    }
    LanguageDto dto = await LanguageQuerier.ReadAsync(language, cancellationToken);

    // TODO(fpion): delete Dictionary if it exists

    language.Delete(ApplicationContext.ActorId);
    await LanguageRepository.SaveAsync(language, cancellationToken);

    return dto;
  }
}
