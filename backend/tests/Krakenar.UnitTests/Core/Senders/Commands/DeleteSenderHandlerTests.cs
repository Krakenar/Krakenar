using Bogus;
using Krakenar.Core.Realms;
using Krakenar.Core.Senders.Events;
using Krakenar.Core.Users;
using Logitar.EventSourcing;
using Moq;
using SenderDto = Krakenar.Contracts.Senders.Sender;

namespace Krakenar.Core.Senders.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class DeleteSenderHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;
  private readonly Faker _faker = new();

  private readonly Mock<IApplicationContext> _applicationContext = new();
  private readonly Mock<ISenderQuerier> _senderQuerier = new();
  private readonly Mock<ISenderRepository> _senderRepository = new();

  private readonly DeleteSenderHandler _handler;

  public DeleteSenderHandlerTests()
  {
    _handler = new(_applicationContext.Object, _senderQuerier.Object, _senderRepository.Object);
  }

  [Fact(DisplayName = "It should delete the sender.")]
  public async Task Given_Sender_When_HandleAsync_Then_Deleted()
  {
    ActorId actorId = ActorId.NewId();
    _applicationContext.SetupGet(x => x.ActorId).Returns(actorId);

    RealmId realmId = RealmId.NewId();
    _applicationContext.SetupGet(x => x.RealmId).Returns(realmId);

    Sender sender = new(new Email(_faker.Person.Email), SenderHelper.GenerateSendGridSettings(), isDefault: false, actorId, SenderId.NewId(realmId));
    _senderRepository.Setup(x => x.LoadAsync(sender.Id, _cancellationToken)).ReturnsAsync(sender);

    SenderDto dto = new();
    _senderQuerier.Setup(x => x.ReadAsync(sender, _cancellationToken)).ReturnsAsync(dto);

    DeleteSender command = new(sender.EntityId);
    SenderDto? result = await _handler.HandleAsync(command, _cancellationToken);
    Assert.NotNull(result);
    Assert.Same(dto, result);

    Assert.True(sender.IsDeleted);
    Assert.Contains(sender.Changes, change => change is SenderDeleted deleted && deleted.ActorId == actorId);

    _senderRepository.Verify(x => x.SaveAsync(sender, _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should return null when the sender was not found.")]
  public async Task Given_NotFound_When_HandleAsync_Then_NullReturned()
  {
    DeleteSender command = new(Guid.Empty);
    Assert.Null(await _handler.HandleAsync(command, _cancellationToken));
  }
}
