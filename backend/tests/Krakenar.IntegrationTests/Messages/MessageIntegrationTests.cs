using Krakenar.Contracts.Messages;
using Krakenar.Contracts.Search;
using Krakenar.Core;
using Krakenar.Core.Messages;
using Krakenar.Core.Senders;
using Krakenar.Core.Templates;
using Microsoft.Extensions.DependencyInjection;
using Email = Krakenar.Core.Users.Email;
using Message = Krakenar.Core.Messages.Message;
using MessageDto = Krakenar.Contracts.Messages.Message;
using Recipient = Krakenar.Core.Messages.Recipient;

namespace Krakenar.Messages;

[Trait(Traits.Category, Categories.Integration)]
public class MessageIntegrationTests : IntegrationTests
{
  private readonly IMessageRepository _messageRepository;
  private readonly IMessageService _messageService;
  private readonly ISenderRepository _senderRepository;
  private readonly ITemplateRepository _templateRepository;

  private Sender _sendGrid = null!;
  private Template _passwordRecovery = null!;

  public MessageIntegrationTests() : base()
  {
    _messageRepository = ServiceProvider.GetRequiredService<IMessageRepository>();
    _messageService = ServiceProvider.GetRequiredService<IMessageService>();
    _senderRepository = ServiceProvider.GetRequiredService<ISenderRepository>();
    _templateRepository = ServiceProvider.GetRequiredService<ITemplateRepository>();
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    _sendGrid = new(new Email(Faker.Internet.Email()), SenderHelper.GenerateSendGridSettings(), isDefault: true, ActorId, SenderId.NewId(Realm.Id))
    {
      DisplayName = new DisplayName(Faker.Company.CompanyName())
    };
    _sendGrid.Update(ActorId);
    await _senderRepository.SaveAsync(_sendGrid);

    UniqueName uniqueName = new(Realm.UniqueNameSettings, "PasswordRecovery");
    Subject subject = new("PasswordRecovery_Subject");
    Content content = Content.Html(@"<div>Click the link below to reset your password:<br />@(Model.Variable(""Token""))</div>");
    _passwordRecovery = new Template(uniqueName, subject, content, ActorId, TemplateId.NewId(Realm.Id))
    {
      DisplayName = new DisplayName("Password Recovery")
    };
    _passwordRecovery.Update(ActorId);
    await _templateRepository.SaveAsync(_passwordRecovery);
  }

  [Fact(DisplayName = "It should read the message by ID.")]
  public async Task Given_Id_When_Read_Then_Found()
  {
    Recipient[] recipients = [new(email: new Email(Faker.Person.Email))];
    Message message = new(_passwordRecovery.Subject, _passwordRecovery.Content, recipients, _sendGrid, _passwordRecovery, messageId: MessageId.NewId(Realm.Id));
    await _messageRepository.SaveAsync(message);

    MessageDto? result = await _messageService.ReadAsync(message.EntityId);
    Assert.NotNull(result);
    Assert.Equal(message.EntityId, result.Id);
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
}
