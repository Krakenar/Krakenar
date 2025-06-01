using Krakenar.Contracts.Messages;
using Krakenar.Contracts.Search;
using Krakenar.Contracts.Senders;
using Krakenar.Contracts.Users;
using Krakenar.Core;
using Krakenar.Core.Dictionaries;
using Krakenar.Core.Encryption;
using Krakenar.Core.Localization;
using Krakenar.Core.Messages;
using Krakenar.Core.Senders;
using Krakenar.Core.Senders.Settings;
using Krakenar.Core.Templates;
using Krakenar.Senders;
using Logitar;
using Logitar.Security.Cryptography;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Email = Krakenar.Core.Users.Email;
using Message = Krakenar.Core.Messages.Message;
using MessageDto = Krakenar.Contracts.Messages.Message;
using Phone = Krakenar.Core.Users.Phone;
using Recipient = Krakenar.Core.Messages.Recipient;
using Sender = Krakenar.Core.Senders.Sender;

namespace Krakenar.Messages;

[Trait(Traits.Category, Categories.Integration)]
public class MessageIntegrationTests : IntegrationTests
{
  private static readonly Locale _canadianFrench = new("fr-CA");
  private static readonly Locale _french = new("fr");

  private readonly IConfiguration _configuration;
  private readonly IDictionaryRepository _dictionaryRepository;
  private readonly IEncryptionManager _encryptionManager;
  private readonly ILanguageQuerier _languageQuerier;
  private readonly ILanguageRepository _languageRepository;
  private readonly IMessageRepository _messageRepository;
  private readonly IMessageService _messageService;
  private readonly ISenderRepository _senderRepository;
  private readonly ITemplateRepository _templateRepository;

  private Sender _sendGrid = null!;
  private Sender _twilio = null!;

  private Template _multiFactorAuthentication = null!;
  private Template _passwordRecovery = null!;

  public MessageIntegrationTests() : base()
  {
    _configuration = ServiceProvider.GetRequiredService<IConfiguration>();
    _dictionaryRepository = ServiceProvider.GetRequiredService<IDictionaryRepository>();
    _encryptionManager = ServiceProvider.GetRequiredService<IEncryptionManager>();
    _languageQuerier = ServiceProvider.GetRequiredService<ILanguageQuerier>();
    _languageRepository = ServiceProvider.GetRequiredService<ILanguageRepository>();
    _messageRepository = ServiceProvider.GetRequiredService<IMessageRepository>();
    _messageService = ServiceProvider.GetRequiredService<IMessageService>();
    _senderRepository = ServiceProvider.GetRequiredService<ISenderRepository>();
    _templateRepository = ServiceProvider.GetRequiredService<ITemplateRepository>();
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    SenderConfiguration senderConfiguration = _configuration.GetSection(SenderConfiguration.SectionKey).Get<SenderConfiguration>() ?? new();

    Email email = new(senderConfiguration.SendGrid.EmailAddress?.CleanTrim() ?? Faker.Internet.Email());
    SendGridSettings sendGridSettings = new(
      _encryptionManager.Encrypt(senderConfiguration.SendGrid.ApiKey?.CleanTrim() ?? SenderHelper.GenerateSendGridApiKey(), Realm.Id).Value);
    _sendGrid = new(email, sendGridSettings, isDefault: true, ActorId, SenderId.NewId(Realm.Id))
    {
      DisplayName = new DisplayName(senderConfiguration.SendGrid.DisplayName?.CleanTrim() ?? Faker.Company.CompanyName())
    };
    _sendGrid.Update(ActorId);

    Phone phone = new(senderConfiguration.Twilio.PhoneNumber?.CleanTrim() ?? "+15148454636", countryCode: "CA");
    TwilioSettings twilioSettings = new(
      _encryptionManager.Encrypt(senderConfiguration.Twilio.AccountSid?.CleanTrim() ?? SenderHelper.GenerateTwilioAccountSid(), Realm.Id).Value,
      _encryptionManager.Encrypt(senderConfiguration.Twilio.AuthenticationToken?.CleanTrim() ?? SenderHelper.GenerateTwilioAuthenticationToken(), Realm.Id).Value);
    _twilio = new(phone, twilioSettings, isDefault: true, ActorId, SenderId.NewId(Realm.Id));

    await _senderRepository.SaveAsync([_sendGrid, _twilio]);

    Content multiFactorAuthenticationContent = Content.PlainText(@"@(Model.Resource(""MultiFactorAuthentication_Body"")) @(Model.Variable(""Code""))");
    _multiFactorAuthentication = new Template(
      new UniqueName(Realm.UniqueNameSettings, "MultiFactorAuthentication"),
      new Subject("MultiFactorAuthentication_Subject"),
      multiFactorAuthenticationContent,
      ActorId,
      TemplateId.NewId(Realm.Id))
    {
      DisplayName = new DisplayName("Multi-Factor Authentication")
    };
    _multiFactorAuthentication.Update(ActorId);

    Content passwordRecoveryContent = Content.Html(await File.ReadAllTextAsync("Templates/PasswordRecovery.cshtml"));
    _passwordRecovery = new Template(
      new UniqueName(Realm.UniqueNameSettings, "PasswordRecovery"),
      new Subject("PasswordRecovery_Subject"),
      passwordRecoveryContent,
      ActorId,
      TemplateId.NewId(Realm.Id))
    {
      DisplayName = new DisplayName("Password Recovery")
    };
    _passwordRecovery.Update(ActorId);

    await _templateRepository.SaveAsync([_multiFactorAuthentication, _passwordRecovery]);

    LanguageId defaultId = await _languageQuerier.FindDefaultIdAsync();
    Language? english = await _languageRepository.LoadAsync(defaultId);
    Assert.NotNull(english);

    Language french = new(_french, isDefault: false, ActorId, LanguageId.NewId(Realm.Id));
    Language canadianFrench = new(_canadianFrench, isDefault: false, ActorId, LanguageId.NewId(Realm.Id));
    await _languageRepository.SaveAsync([french, canadianFrench]);

    Dictionary englishDictionary = new(english, ActorId, DictionaryId.NewId(Realm.Id));
    englishDictionary.SetEntry(new Identifier("Team"), "The Logitar Team");
    englishDictionary.SetEntry(new Identifier("PasswordRecovery_LostYourPassword"), "It seems you have lost your password.");
    englishDictionary.SetEntry(new Identifier("MultiFactorAuthentication_Body"), "Your Multi-Factor Authentication code has arrived! Do not disclose it to anyone: ");
    englishDictionary.SetEntry(new Identifier("MultiFactorAuthentication_Subject"), "Your Multi-Factor Authentication code has arrived!");
    englishDictionary.Update(ActorId);

    Dictionary frenchDictionary = new(french, ActorId, DictionaryId.NewId(Realm.Id));
    frenchDictionary.SetEntry(new Identifier("PasswordRecovery_Subject"), "Réinitialiser votre mot de passe");
    frenchDictionary.SetEntry(new Identifier("PasswordRecovery_LostYourPassword"), "Il semblerait que vous avez perdu votre mot de passe.");
    frenchDictionary.SetEntry(new Identifier("PasswordRecovery_ClickLink"), "Cliquez sur le lien ci-dessous afin de le réinitialiser.");
    frenchDictionary.SetEntry(new Identifier("PasswordRecovery_Otherwise"), "S’il s’agit d’une erreur de notre part, veuillez supprimer ce message.");
    frenchDictionary.SetEntry(new Identifier("Cordially"), "Sincèrement,");
    frenchDictionary.Update(ActorId);

    Dictionary canadianFrenchDictionary = new(canadianFrench, ActorId, DictionaryId.NewId(Realm.Id));
    canadianFrenchDictionary.SetEntry(new Identifier("Hello"), "Bonjour !");
    canadianFrenchDictionary.SetEntry(new Identifier("Cordially"), "Cordialement,");
    canadianFrenchDictionary.Update(ActorId);

    await _dictionaryRepository.SaveAsync([englishDictionary, frenchDictionary, canadianFrenchDictionary]);
  }

  [Fact(DisplayName = "It should read the message by ID.")]
  public async Task Given_Id_When_Read_Then_Found()
  {
    Content content = new(_passwordRecovery.Content.Type, _encryptionManager.Encrypt(_passwordRecovery.Content.Text, Realm.Id).Value);
    Recipient[] recipients = [new(email: new Email(Faker.Person.Email))];
    string id = Guid.NewGuid().ToString();
    Dictionary<string, string> variables = new()
    {
      ["Id"] = _encryptionManager.Encrypt(id, Realm.Id).Value
    };
    Message message = new(_passwordRecovery.Subject, content, recipients, _sendGrid, _passwordRecovery, variables: variables.AsReadOnly(), messageId: MessageId.NewId(Realm.Id));
    await _messageRepository.SaveAsync(message);

    MessageDto? result = await _messageService.ReadAsync(message.EntityId);
    Assert.NotNull(result);
    Assert.Equal(message.EntityId, result.Id);
    Assert.Equal(_passwordRecovery.Content.Type, result.Body.Type);
    Assert.Equal(_passwordRecovery.Content.Text, result.Body.Text);

    Variable variable = Assert.Single(result.Variables);
    Assert.Equal("Id", variable.Key);
    Assert.Equal(id, variable.Value);
  }

  [Fact(DisplayName = "It should return null when the message cannot be found.")]
  public async Task Given_NotFound_When_Read_Then_NullReturned()
  {
    Assert.Null(await _messageService.ReadAsync(Guid.Empty));
  }

  [Fact(DisplayName = "It should return the correct search results.")]
  public async Task Given_Messages_When_Search_Then_CorrectResults()
  {
    Recipient[] recipients = [new(email: new Email(Faker.Person.Email))];
    Message message1 = new(_passwordRecovery.Subject, _passwordRecovery.Content, recipients, _sendGrid, _passwordRecovery, messageId: MessageId.NewId(Realm.Id));
    Message message2 = new(_passwordRecovery.Subject, _passwordRecovery.Content, recipients, _sendGrid, _passwordRecovery, messageId: MessageId.NewId(Realm.Id));
    Message message3 = new(new Subject("Reset your password"), _passwordRecovery.Content, recipients, _sendGrid, _passwordRecovery, messageId: MessageId.NewId(Realm.Id));
    Message message4 = new(new Subject("Réinitialiser votre mot de passe"), _passwordRecovery.Content, recipients, _sendGrid, _passwordRecovery, messageId: MessageId.NewId(Realm.Id));
    await _messageRepository.SaveAsync([message1, message2, message3, message4]);

    SearchMessagesPayload payload = new()
    {
      Ids = [message2.EntityId, message3.EntityId, message4.EntityId, Guid.Empty],
      Search = new TextSearch([new SearchTerm("%_Subject"), new SearchTerm("reset%")], SearchOperator.Or),
      Sort = [new MessageSortOption(MessageSort.Subject, isDescending: true)],
      Skip = 1,
      Limit = 1
    };
    SearchResults<MessageDto> results = await _messageService.SearchAsync(payload);
    Assert.Equal(2, results.Total);

    MessageDto message = Assert.Single(results.Items);
    Assert.Equal(message2.EntityId, message.Id);
  }

  [Fact(DisplayName = "It should return the correct search results (IsDemo).")]
  public async Task Given_IsDemo_When_Search_Then_CorrectResults()
  {
    Recipient[] recipients = [new(email: new Email(Faker.Person.Email))];
    Message demo = new(_passwordRecovery.Subject, _passwordRecovery.Content, recipients, _sendGrid, _passwordRecovery, isDemo: true, messageId: MessageId.NewId(Realm.Id));
    Message notDemo = new(_passwordRecovery.Subject, _passwordRecovery.Content, recipients, _sendGrid, _passwordRecovery, messageId: MessageId.NewId(Realm.Id));
    await _messageRepository.SaveAsync([demo, notDemo]);

    SearchMessagesPayload payload = new()
    {
      IsDemo = true
    };
    SearchResults<MessageDto> results = await _messageService.SearchAsync(payload);
    Assert.Equal(1, results.Total);

    MessageDto message = Assert.Single(results.Items);
    Assert.Equal(demo.EntityId, message.Id);
  }

  [Fact(DisplayName = "It should return the correct search results (Status).")]
  public async Task Given_Status_When_Search_Then_CorrectResults()
  {
    Recipient[] recipients = [new(email: new Email(Faker.Person.Email))];
    Message unsent = new(_multiFactorAuthentication.Subject, _multiFactorAuthentication.Content, recipients, _sendGrid, _multiFactorAuthentication, messageId: MessageId.NewId(Realm.Id));
    Message sent = new(_passwordRecovery.Subject, _passwordRecovery.Content, recipients, _sendGrid, _passwordRecovery, messageId: MessageId.NewId(Realm.Id));
    Message failed = new(_multiFactorAuthentication.Subject, _multiFactorAuthentication.Content, recipients, _sendGrid, _multiFactorAuthentication, messageId: MessageId.NewId(Realm.Id));
    failed.Fail(ActorId);
    await _messageRepository.SaveAsync([unsent, sent, failed]);

    SearchMessagesPayload payload = new()
    {
      Status = MessageStatus.Failed
    };
    SearchResults<MessageDto> results = await _messageService.SearchAsync(payload);
    Assert.Equal(1, results.Total);

    MessageDto message = Assert.Single(results.Items);
    Assert.Equal(failed.EntityId, message.Id);
  }

  [Fact(DisplayName = "It should return the correct search results (Template).")]
  public async Task Given_Template_When_Search_Then_CorrectResults()
  {
    Recipient[] recipients = [new(email: new Email(Faker.Person.Email))];
    Message multiFactorAuthentication = new(_multiFactorAuthentication.Subject, _multiFactorAuthentication.Content, recipients, _sendGrid, _multiFactorAuthentication, messageId: MessageId.NewId(Realm.Id));
    Message passwordRecovery = new(_passwordRecovery.Subject, _passwordRecovery.Content, recipients, _sendGrid, _passwordRecovery, messageId: MessageId.NewId(Realm.Id));
    await _messageRepository.SaveAsync([multiFactorAuthentication, passwordRecovery]);

    SearchMessagesPayload payload = new()
    {
      TemplateId = _passwordRecovery.EntityId
    };
    SearchResults<MessageDto> results = await _messageService.SearchAsync(payload);
    Assert.Equal(1, results.Total);

    MessageDto message = Assert.Single(results.Items);
    Assert.Equal(passwordRecovery.EntityId, message.Id);
  }

  [Fact(DisplayName = "It should send an email message from SendGrid.")]
  public async Task Given_SendGrid_When_Send_Then_MessageSent()
  {
    SendMessagePayload payload = new()
    {
      Sender = SenderKind.Email.ToString(),
      Template = $"  {_passwordRecovery.UniqueName.Value}  ",
      IgnoreUserLocale = true,
      Locale = _canadianFrench.Code,
      IsDemo = true
    };

    RecipientsConfiguration recipients = _configuration.GetSection(RecipientsConfiguration.SectionKey).Get<RecipientsConfiguration>() ?? new();
    foreach (EmailRecipient recipient in recipients.Email)
    {
      payload.Recipients.Add(new RecipientPayload(new EmailPayload(recipient.Address), recipient.DisplayName, recipient.Type));
    }

    string token = Guid.NewGuid().ToString();
    payload.Variables.Add(new Variable("Token", token));

    SentMessages sentMessages = await _messageService.SendAsync(payload);
    Guid entityId = Assert.Single(sentMessages.Ids);

    MessageId messageId = new(entityId, Realm.Id);
    Message? message = await _messageRepository.LoadAsync(messageId);
    Assert.NotNull(message);

    Assert.Equal(messageId, message.Id);
    Assert.Equal(2, message.Version);
    Assert.Equal(ActorId, message.CreatedBy);
    Assert.Equal(DateTime.UtcNow, message.CreatedOn.AsUniversalTime(), TimeSpan.FromSeconds(10));
    Assert.Equal(ActorId, message.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, message.UpdatedOn.AsUniversalTime(), TimeSpan.FromSeconds(10));

    Assert.Equal("Réinitialiser votre mot de passe", message.Subject.Value);
    Assert.Equal(_passwordRecovery.Content.Type, message.Body.Type);
    Assert.Contains($@"lang=""{_canadianFrench.Code}""", message.Body.Text);
    Assert.Contains(HttpUtility.HtmlEncode("Bonjour !"), message.Body.Text);
    Assert.Contains(HttpUtility.HtmlEncode("Il semblerait que vous avez perdu votre mot de passe."), message.Body.Text);
    Assert.Contains(HttpUtility.HtmlEncode("Cliquez sur le lien ci-dessous afin de le réinitialiser."), message.Body.Text);
    Assert.Contains(HttpUtility.HtmlEncode($"https://www.francispion.ca/password/reset?token={token}"), message.Body.Text);
    Assert.Contains(HttpUtility.HtmlEncode("S’il s’agit d’une erreur de notre part, veuillez supprimer ce message."), message.Body.Text);
    Assert.Contains(HttpUtility.HtmlEncode("Cordialement,"), message.Body.Text);
    Assert.Contains(HttpUtility.HtmlEncode("The Logitar Team"), message.Body.Text);

    Assert.Equal(payload.Recipients.Count, message.Recipients.Count);
    foreach (RecipientPayload recipient in payload.Recipients)
    {
      Assert.Contains(message.Recipients, r => r.Type == recipient.Type
        && r.Email?.Address == recipient.Email?.Address
        && r.Phone?.Number == recipient.Phone?.Number
        && r.DisplayName?.Value == recipient.DisplayName
        && r.UserId?.EntityId == recipient.UserId);
    }

    Assert.Equal(_sendGrid.Id, message.Sender.Id);
    Assert.Equal(_sendGrid.IsDefault, message.Sender.IsDefault);
    Assert.Equal(_sendGrid.Email, message.Sender.Email);
    Assert.Equal(_sendGrid.Phone, message.Sender.Phone);
    Assert.Equal(_sendGrid.DisplayName, message.Sender.DisplayName);
    Assert.Equal(_sendGrid.Provider, message.Sender.Provider);

    Assert.Equal(_passwordRecovery.Id, message.Template.Id);
    Assert.Equal(_passwordRecovery.UniqueName, message.Template.UniqueName);
    Assert.Equal(_passwordRecovery.DisplayName, message.Template.DisplayName);

    Assert.Equal(payload.IgnoreUserLocale, message.IgnoreUserLocale);
    Assert.Equal(payload.Locale, message.Locale?.Code);

    Assert.Equal(payload.Variables.Count, message.Variables.Count);
    foreach (Variable variable in payload.Variables)
    {
      Assert.Contains(message.Variables, v => v.Key == variable.Key && v.Value == variable.Value);
    }

    Assert.Equal(payload.IsDemo, message.IsDemo);

    Assert.Equal(MessageStatus.Succeeded, message.Status);
    Assert.Equal(5, message.Results.Count);
    Assert.Contains(message.Results, r => r.Key == "ReasonPhrase" && r.Value == "Accepted");
    Assert.Contains(message.Results, r => r.Key == "Version" && r.Value == "1.1");
    Assert.Contains(message.Results, r => r.Key == "Headers");
    Assert.Contains(message.Results, r => r.Key == "Status");
    Assert.Contains(message.Results, r => r.Key == "TrailingHeaders");
  }

  [Fact(DisplayName = "It should send a SMS message from Twilio.")]
  public async Task Given_Twilio_When_Send_Then_MessageSent()
  {
    SendMessagePayload payload = new()
    {
      Sender = SenderKind.Phone.ToString(),
      Template = _multiFactorAuthentication.EntityId.ToString(),
      IsDemo = true
    };

    RecipientsConfiguration recipients = _configuration.GetSection(RecipientsConfiguration.SectionKey).Get<RecipientsConfiguration>() ?? new();
    foreach (PhoneRecipient recipient in recipients.Phone)
    {
      payload.Recipients.Add(new RecipientPayload(new PhonePayload(recipient.Number, countryCode: "CA"), recipient.Type));
    }

    string code = RandomStringGenerator.GetString("1234567890", count: 6);
    payload.Variables.Add(new Variable("Code", code));

    SentMessages sentMessages = await _messageService.SendAsync(payload);
    Guid entityId = Assert.Single(sentMessages.Ids);

    MessageId messageId = new(entityId, Realm.Id);
    Message? message = await _messageRepository.LoadAsync(messageId);
    Assert.NotNull(message);

    Assert.Equal(messageId, message.Id);
    Assert.Equal(2, message.Version);
    Assert.Equal(ActorId, message.CreatedBy);
    Assert.Equal(DateTime.UtcNow, message.CreatedOn.AsUniversalTime(), TimeSpan.FromSeconds(10));
    Assert.Equal(ActorId, message.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, message.UpdatedOn.AsUniversalTime(), TimeSpan.FromSeconds(10));

    Assert.Equal("Your Multi-Factor Authentication code has arrived!", message.Subject.Value);
    Assert.Equal(_multiFactorAuthentication.Content.Type, message.Body.Type);
    Assert.Equal($"Your Multi-Factor Authentication code has arrived! Do not disclose it to anyone: {code}", message.Body.Text);

    Assert.Equal(payload.Recipients.Count, message.Recipients.Count);
    foreach (RecipientPayload recipient in payload.Recipients)
    {
      Assert.Contains(message.Recipients, r => r.Type == recipient.Type
        && r.Email?.Address == recipient.Email?.Address
        && r.Phone?.Number == recipient.Phone?.Number
        && r.DisplayName?.Value == recipient.DisplayName
        && r.UserId?.EntityId == recipient.UserId);
    }

    Assert.Equal(_twilio.Id, message.Sender.Id);
    Assert.Equal(_twilio.IsDefault, message.Sender.IsDefault);
    Assert.Equal(_twilio.Email, message.Sender.Email);
    Assert.Equal(_twilio.Phone, message.Sender.Phone);
    Assert.Equal(_twilio.DisplayName, message.Sender.DisplayName);
    Assert.Equal(_twilio.Provider, message.Sender.Provider);

    Assert.Equal(_multiFactorAuthentication.Id, message.Template.Id);
    Assert.Equal(_multiFactorAuthentication.UniqueName, message.Template.UniqueName);
    Assert.Equal(_multiFactorAuthentication.DisplayName, message.Template.DisplayName);

    Assert.Equal(payload.IgnoreUserLocale, message.IgnoreUserLocale);
    Assert.Equal(payload.Locale, message.Locale?.Code);

    Assert.Equal(payload.Variables.Count, message.Variables.Count);
    foreach (Variable variable in payload.Variables)
    {
      Assert.Contains(message.Variables, v => v.Key == variable.Key && v.Value == variable.Value);
    }

    Assert.Equal(payload.IsDemo, message.IsDemo);

    Assert.Equal(MessageStatus.Succeeded, message.Status);
    Assert.Equal(6, message.Results.Count);
    Assert.Contains(message.Results, r => r.Key == "ReasonPhrase" && r.Value == "Created");
    Assert.Contains(message.Results, r => r.Key == "Version" && r.Value == "1.1");
    Assert.Contains(message.Results, r => r.Key == "Headers");
    Assert.Contains(message.Results, r => r.Key == "JsonContent");
    Assert.Contains(message.Results, r => r.Key == "Status");
    Assert.Contains(message.Results, r => r.Key == "TrailingHeaders");
  }
}
