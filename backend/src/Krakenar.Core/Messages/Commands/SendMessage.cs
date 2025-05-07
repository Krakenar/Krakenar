using FluentValidation;
using Krakenar.Contracts.Dictionaries;
using Krakenar.Contracts.Messages;
using Krakenar.Contracts.Search;
using Krakenar.Contracts.Senders;
using Krakenar.Core.Dictionaries;
using Krakenar.Core.Localization;
using Krakenar.Core.Messages.Validators;
using Krakenar.Core.Realms;
using Krakenar.Core.Senders;
using Krakenar.Core.Templates;
using Krakenar.Core.Users;
using Logitar.EventSourcing;
using DictionaryDto = Krakenar.Contracts.Dictionaries.Dictionary;
using MediaTypeNames = System.Net.Mime.MediaTypeNames;
using Sender = Krakenar.Core.Senders.Sender;

namespace Krakenar.Core.Messages.Commands;

public record SendMessage(SendMessagePayload Payload) : ICommand<SentMessages>;

/// <exception cref="InvalidSmsMessageContentTypeException"></exception>
/// <exception cref="MissingRecipientContactsException"></exception>
/// <exception cref="SenderNotFoundException"></exception>
/// <exception cref="TemplateNotFoundException"></exception>
/// <exception cref="UsersNotFoundException"></exception>
/// <exception cref="ValidationException"></exception>
public class SendMessageHandler : ICommandHandler<SendMessage, SentMessages>
{
  protected virtual IApplicationContext ApplicationContext { get; }
  protected virtual IDictionaryQuerier DictionaryQuerier { get; }
  protected virtual ILanguageQuerier LanguageQuerier { get; }
  protected virtual IMessageManager MessageManager { get; }
  protected virtual IMessageQuerier MessageQuerier { get; }
  protected virtual IMessageRepository MessageRepository { get; }
  protected virtual ISenderManager SenderManager { get; }
  protected virtual ITemplateManager TemplateManager { get; }
  protected virtual IUserRepository UserRepository { get; }

  public SendMessageHandler(
    IApplicationContext applicationContext,
    IDictionaryQuerier dictionaryQuerier,
    ILanguageQuerier languageQuerier,
    IMessageManager messageManager,
    IMessageQuerier messageQuerier,
    IMessageRepository messageRepository,
    ISenderManager senderManager,
    ITemplateManager templateManager,
    IUserRepository userRepository)
  {
    ApplicationContext = applicationContext;
    DictionaryQuerier = dictionaryQuerier;
    LanguageQuerier = languageQuerier;
    MessageManager = messageManager;
    MessageQuerier = messageQuerier;
    MessageRepository = messageRepository;
    SenderManager = senderManager;
    TemplateManager = templateManager;
    UserRepository = userRepository;
  }

  public virtual async Task<SentMessages> HandleAsync(SendMessage command, CancellationToken cancellationToken)
  {
    SendMessagePayload payload = command.Payload;
    Sender sender = await SenderManager.FindAsync(payload.Sender, nameof(payload.Sender), cancellationToken);
    new SendMessageValidator(sender.Kind).ValidateAndThrow(payload);

    Template template = await TemplateManager.FindAsync(payload.Template, nameof(payload.Template), cancellationToken);
    if (sender.Kind == SenderKind.Phone && template.Content.Type != MediaTypeNames.Text.Plain)
    {
      throw new InvalidSmsMessageContentTypeException(template.Content.Type, nameof(payload.Template));
    }

    ActorId? actorId = ApplicationContext.ActorId;
    RealmId? realmId = ApplicationContext.RealmId;
    Recipients allRecipients = await ResolveRecipientsAsync(payload, realmId, sender.Kind, cancellationToken);

    IReadOnlyDictionary<Locale, DictionaryDto> dictionariesByLocale = await LoadDictionariesAsync(cancellationToken);
    bool ignoreUserLocale = payload.IgnoreUserLocale;
    Locale defaultLocale = await LanguageQuerier.FindDefaultLocaleAsync(cancellationToken);
    Locale? targetLocale = Locale.TryCreate(payload.Locale);
    Dictionaries defaultDictionaries = new(dictionariesByLocale, defaultLocale, targetLocale);

    Variables variables = new(payload.Variables);
    IReadOnlyDictionary<string, string> variableDictionary = variables.AsDictionary();

    List<Message> messages = new(capacity: allRecipients.To.Count);
    foreach (Recipient recipient in allRecipients.To)
    {
      MessageId messageId = MessageId.NewId(realmId);

      Locale? locale;
      Dictionaries dictionaries;
      if (payload.IgnoreUserLocale || recipient.User?.Locale is null)
      {
        locale = targetLocale;
        dictionaries = defaultDictionaries;
      }
      else
      {
        locale = recipient.User.Locale;
        dictionaries = new Dictionaries(dictionariesByLocale, defaultLocale, locale);
      }

      Subject subject = new(dictionaries.Translate(template.Subject.Value));
      Content body = await MessageManager.CompileAsync(messageId, template, dictionaries, locale, recipient.User, variables, cancellationToken);
      IReadOnlyCollection<Recipient> recipients = [recipient, .. allRecipients.CC, .. allRecipients.Bcc];

      Message message = new(subject, body, recipients, sender, template, ignoreUserLocale, locale, variableDictionary, payload.IsDemo, actorId, messageId);
      messages.Add(message);

      await MessageManager.SendAsync(message, sender, actorId, cancellationToken);
    }
    await MessageRepository.SaveAsync(messages, cancellationToken);

    return new SentMessages(messages.Select(x => x.EntityId));
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

  protected virtual async Task<Recipients> ResolveRecipientsAsync(SendMessagePayload payload, RealmId? realmId, SenderKind senderKind, CancellationToken cancellationToken)
  {
    List<Recipient> recipients = new(capacity: payload.Recipients.Count);

    HashSet<UserId> userIds = new(recipients.Capacity);
    foreach (RecipientPayload recipient in payload.Recipients)
    {
      if (recipient.UserId.HasValue)
      {
        UserId userId = new(recipient.UserId.Value, realmId);
        userIds.Add(userId);
      }
    }

    Dictionary<Guid, User> users = new(recipients.Capacity);
    if (userIds.Count > 0)
    {
      IReadOnlyCollection<User> foundUsers = await UserRepository.LoadAsync(userIds, cancellationToken);
      foreach (User user in foundUsers)
      {
        users[user.EntityId] = user;
      }
    }

    List<Guid> missingUsers = new(recipients.Capacity);
    List<Guid> missingContacts = new(recipients.Capacity);
    foreach (RecipientPayload recipient in payload.Recipients)
    {
      if (recipient.UserId.HasValue)
      {
        if (users.TryGetValue(recipient.UserId.Value, out User? user))
        {
          switch (senderKind)
          {
            case SenderKind.Email:
              if (user.Email is null)
              {
                missingContacts.Add(recipient.UserId.Value);
                continue;
              }
              break;
            case SenderKind.Phone:
              if (user.Phone is null)
              {
                missingContacts.Add(recipient.UserId.Value);
                continue;
              }
              break;
            default:
              throw new ArgumentException($"The sender kind '{senderKind}' is not supported.", nameof(senderKind));
          }

          recipients.Add(new Recipient(user, recipient.Type));
        }
        else
        {
          missingUsers.Add(recipient.UserId.Value);
        }
      }
      else
      {
        recipients.Add(new Recipient(recipient.Type, recipient.Address, recipient.DisplayName, recipient.PhoneNumber));
      }
    }
    if (missingUsers.Count > 0)
    {
      throw new UsersNotFoundException(realmId, missingUsers, nameof(payload.Recipients));
    }
    else if (missingContacts.Count > 0)
    {
      throw new MissingRecipientContactsException(realmId, missingContacts, nameof(payload.Recipients));
    }

    return new Recipients(recipients);
  }
}
