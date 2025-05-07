using FluentValidation;
using Krakenar.Contracts.Dictionaries;
using Krakenar.Contracts.Messages;
using Krakenar.Contracts.Search;
using Krakenar.Core.Dictionaries;
using Krakenar.Core.Localization;
using Krakenar.Core.Messages.Validators;
using Krakenar.Core.Senders;
using Krakenar.Core.Templates;
using DictionaryDto = Krakenar.Contracts.Dictionaries.Dictionary;
using MediaTypeNames = System.Net.Mime.MediaTypeNames;

namespace Krakenar.Core.Messages.Commands;

public record SendMessage(SendMessagePayload Payload) : ICommand<SentMessages>;

/// <exception cref="InvalidSmsMessageContentTypeException"></exception>
/// <exception cref="SenderNotFoundException"></exception>
/// <exception cref="TemplateNotFoundException"></exception>
/// <exception cref="ValidationException"></exception>
public class SendMessageHandler : ICommandHandler<SendMessage, SentMessages>
{
  protected virtual IApplicationContext ApplicationContext { get; }
  protected virtual IDictionaryQuerier DictionaryQuerier { get; }
  protected virtual ILanguageQuerier LanguageQuerier { get; }
  protected virtual IMessageQuerier MessageQuerier { get; }
  protected virtual ISenderManager SenderManager { get; }
  protected virtual ITemplateManager TemplateManager { get; }

  public SendMessageHandler(
    IApplicationContext applicationContext,
    IDictionaryQuerier dictionaryQuerier,
    ILanguageQuerier languageQuerier,
    IMessageQuerier messageQuerier,
    ISenderManager senderManager,
    ITemplateManager templateManager)
  {
    ApplicationContext = applicationContext;
    DictionaryQuerier = dictionaryQuerier;
    LanguageQuerier = languageQuerier;
    MessageQuerier = messageQuerier;
    SenderManager = senderManager;
    TemplateManager = templateManager;
  }

  public virtual async Task<SentMessages> HandleAsync(SendMessage command, CancellationToken cancellationToken)
  {
    SendMessagePayload payload = command.Payload;
    new SendMessageValidator().ValidateAndThrow(payload);

    Sender sender = await SenderManager.FindAsync(payload.Sender, nameof(payload.Sender), cancellationToken);
    Template template = await TemplateManager.FindAsync(payload.Template, nameof(payload.Template), cancellationToken);
    if (sender.Kind == Contracts.Senders.SenderKind.Phone && template.Content.Type != MediaTypeNames.Text.Plain)
    {
      throw new InvalidSmsMessageContentTypeException(template.Content.Type, nameof(payload.Template));
    }

    IReadOnlyDictionary<Locale, DictionaryDto> dictionaries = await LoadDictionariesAsync(cancellationToken);
    bool ignoreUserLocale = payload.IgnoreUserLocale;
    Locale defaultLocale = await LanguageQuerier.FindDefaultLocaleAsync(cancellationToken);
    Locale? targetLocale = Locale.TryCreate(payload.Locale);
    Dictionaries defaultDictionaries = new(dictionaries, defaultLocale, targetLocale);

    Variables variables = new(payload.Variables);
    IReadOnlyDictionary<string, string> variableDictionary = variables.AsDictionary();

    throw new NotImplementedException(); // TODO(fpion): implement
  }

  protected virtual async Task<IReadOnlyDictionary<Locale, DictionaryDto>> LoadDictionariesAsync(CancellationToken cancellationToken)
  {
    SearchResults<DictionaryDto> results = await DictionaryQuerier.SearchAsync(new SearchDictionariesPayload(), cancellationToken);
    Dictionary<Locale, DictionaryDto> dictionaries = new(capacity: results.Items.Count);
    foreach (DictionaryDto dictionary in results.Items)
    {
      Locale locale = new(dictionary.Language.Locale.Code);
      dictionaries[locale] = dictionary;
    }
    return dictionaries.AsReadOnly();
  }
}
