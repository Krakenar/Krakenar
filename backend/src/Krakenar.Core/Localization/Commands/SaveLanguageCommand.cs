using Krakenar.Core.Localization.Events;
using MediatR;

namespace Krakenar.Core.Localization.Commands;

public record SaveLanguageCommand(Language Language) : IRequest;

internal class SaveLanguageCommandHandler : IRequestHandler<SaveLanguageCommand>
{
  private readonly ILanguageQuerier _languageQuerier;
  private readonly ILanguageRepository _languageRepository;

  public SaveLanguageCommandHandler(ILanguageQuerier languageQuerier, ILanguageRepository languageRepository)
  {
    _languageQuerier = languageQuerier;
    _languageRepository = languageRepository;
  }

  public async Task Handle(SaveLanguageCommand command, CancellationToken cancellationToken)
  {
    Language language = command.Language;

    bool hasLocaleChanged = language.Changes.Any(change => change is LanguageCreated);
    if (hasLocaleChanged)
    {
      LanguageId? conflictId = await _languageQuerier.FindIdAsync(language.Locale, cancellationToken);
      if (conflictId.HasValue && !conflictId.Value.Equals(language.Id))
      {
        throw new NotImplementedException(); // TODO(fpion): implement
      }
    }

    await _languageRepository.SaveAsync(language, cancellationToken);
  }
}
