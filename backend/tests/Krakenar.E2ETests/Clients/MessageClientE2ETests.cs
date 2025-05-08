using Bogus;
using Krakenar.Client.Messages;
using Krakenar.Client.Senders;
using Krakenar.Client.Templates;
using Krakenar.Contracts.Messages;
using Krakenar.Contracts.Search;
using Krakenar.Contracts.Senders;
using Krakenar.Contracts.Senders.Settings;
using Krakenar.Contracts.Templates;
using Krakenar.Contracts.Users;
using Logitar;
using Logitar.Security.Cryptography;
using Microsoft.Extensions.Configuration;

namespace Krakenar.Clients;

[Trait(Traits.Category, Categories.EndToEnd)]
public class MessageClientE2ETests : E2ETests
{
  private readonly CancellationToken _cancellationToken = default;
  private readonly Faker _faker = new();

  public MessageClientE2ETests() : base()
  {
  }

  [Fact(DisplayName = "Messages should be managed correctly through the API client.")]
  public async Task Given_Realm_When_Client_Then_Success()
  {
    await InitializeRealmAsync(_cancellationToken);

    MessagingConfiguration configuration = base.Configuration.GetSection(MessagingConfiguration.SectionKey).Get<MessagingConfiguration>() ?? new();

    KrakenarSettings.Realm = Realm.UniqueSlug;
    using HttpClient messageClient = new();
    MessageClient messages = new(messageClient, KrakenarSettings);
    using HttpClient senderClient = new();
    SenderClient senders = new(senderClient, KrakenarSettings);
    using HttpClient templateClient = new();
    TemplateClient templates = new(templateClient, KrakenarSettings);

    Guid senderId = Guid.Parse("383cd630-70ba-488e-bbf1-aefc443f72bb");
    CreateOrReplaceSenderPayload createOrReplaceSender = new()
    {
      Email = new EmailPayload(configuration.SendGrid.EmailAddress?.CleanTrim() ?? _faker.Internet.Email()),
      DisplayName = configuration.SendGrid.DisplayName?.CleanTrim() ?? _faker.Company.CompanyName(),
      SendGrid = new SendGridSettings(configuration.SendGrid.ApiKey?.CleanTrim() ?? SenderHelper.GenerateSendGridApiKey())
    };
    CreateOrReplaceSenderResult senderResult = await senders.CreateOrReplaceAsync(createOrReplaceSender, senderId, version: null, _cancellationToken);
    Assert.NotNull(senderResult.Sender);

    Guid templateId = Guid.Parse("f7a676fd-24fc-4efd-ac09-d7a8924f475e");
    CreateOrReplaceTemplatePayload createOrReplaceTemplate = new()
    {
      UniqueName = "MultiFactorAuthentication",
      DisplayName = "Multi-Factor Authentication",
      Subject = "Your One-Time Password Code has arrived!",
      Content = Content.PlainText(@"Your One-Time Password Code has arrived!\n@(Model.Variable(""Code""))")
    };
    CreateOrReplaceTemplateResult templateResult = await templates.CreateOrReplaceAsync(createOrReplaceTemplate, templateId, version: null, _cancellationToken);
    Assert.NotNull(templateResult.Template);

    SendMessagePayload payload = new()
    {
      Sender = senderId.ToString(),
      Template = createOrReplaceTemplate.UniqueName,
      Locale = "en",
      IsDemo = true
    };
    payload.Recipients.Add(new RecipientPayload
    {
      Email = new EmailPayload(configuration.Recipient.EmailAddress?.CleanTrim() ?? _faker.Person.Email),
      DisplayName = configuration.Recipient.DisplayName?.CleanTrim() ?? _faker.Person.FullName
    });
    payload.Variables.Add(new Variable("Code", RandomStringGenerator.GetString("1234567890", count: 6)));

    SentMessages sentMessages = await messages.SendAsync(payload, _cancellationToken);
    Guid messageId = Assert.Single(sentMessages.Ids);

    Message? message = await messages.ReadAsync(messageId, _cancellationToken);
    Assert.NotNull(message);
    Assert.Equal(messageId, message.Id);
    Assert.Equal(payload.Recipients.Count, message.Recipients.Count);
    Assert.Equal(senderId, message.Sender.Id);
    Assert.Equal(templateId, message.Template.Id);
    Assert.Equal(payload.IgnoreUserLocale, message.IgnoreUserLocale);
    Assert.Equal(payload.Locale, message.Locale?.Code);
    Assert.Equal(payload.Variables, message.Variables);
    Assert.Equal(payload.IsDemo, message.IsDemo);
    Assert.NotEqual(MessageStatus.Unsent, message.Status);
    Assert.NotEmpty(message.Results);

    SearchMessagesPayload search = new()
    {
      Ids = [messageId],
      IsDemo = true,
      Search = new TextSearch([new SearchTerm("%password%")]),
      TemplateId = templateId
    };
    SearchResults<Message> results = await messages.SearchAsync(search, _cancellationToken);
    Assert.Equal(1, results.Total);
    message = Assert.Single(results.Items);
    Assert.Equal(messageId, message.Id);
  }
}
