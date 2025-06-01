using Bogus;
using Krakenar.Contracts.Senders;
using Krakenar.Core.Encryption;
using Krakenar.Core.Realms;
using Krakenar.Core.Senders.Events;
using Krakenar.Core.Users;
using Logitar.EventSourcing;
using Moq;
using SenderDto = Krakenar.Contracts.Senders.Sender;

namespace Krakenar.Core.Senders.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class SetDefaultSenderHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;
  private readonly Faker _faker = new();

  private readonly Mock<IApplicationContext> _applicationContext = new();
  private readonly Mock<IEncryptionManager> _encryptionManager = new();
  private readonly Mock<ISenderQuerier> _senderQuerier = new();
  private readonly Mock<ISenderRepository> _senderRepository = new();

  private readonly SetDefaultSenderHandler _handler;

  public SetDefaultSenderHandlerTests()
  {
    _handler = new(_applicationContext.Object, _encryptionManager.Object, _senderQuerier.Object, _senderRepository.Object);
  }

  [Fact(DisplayName = "It should not do anything when the sender is already default.")]
  public async Task Given_IsDefault_When_HandleAsync_Then_Nothing()
  {
    Sender sender = new(new Email(_faker.Person.Email), SenderHelper.GenerateSendGridSettings(), isDefault: true);
    sender.ClearChanges();
    _senderRepository.Setup(x => x.LoadAsync(sender.Id, _cancellationToken)).ReturnsAsync(sender);

    SenderDto dto = new();
    _senderQuerier.Setup(x => x.ReadAsync(sender, _cancellationToken)).ReturnsAsync(dto);

    SetDefaultSender command = new(sender.EntityId);
    SenderDto? result = await _handler.HandleAsync(command, _cancellationToken);
    Assert.NotNull(result);
    Assert.Same(dto, result);

    Assert.True(sender.IsDefault);
    Assert.False(sender.HasChanges);
    Assert.Empty(sender.Changes);

    _senderQuerier.Verify(x => x.FindDefaultIdAsync(It.IsAny<SenderKind>(), It.IsAny<CancellationToken>()), Times.Never);
    _senderRepository.Verify(x => x.SaveAsync(It.IsAny<IEnumerable<Sender>>(), _cancellationToken), Times.Never);
  }

  [Fact(DisplayName = "It should return null when the sender was not found.")]
  public async Task Given_NotFound_When_HandleAsync_Then_NullReturned()
  {
    SetDefaultSender command = new(Guid.Empty);
    Assert.Null(await _handler.HandleAsync(command, _cancellationToken));
  }

  [Fact(DisplayName = "It should set the sender default.")]
  public async Task Given_NotDefault_When_HandleAsync_Then_SetDefault()
  {
    ActorId actorId = ActorId.NewId();
    _applicationContext.SetupGet(x => x.ActorId).Returns(actorId);

    RealmId realmId = RealmId.NewId();
    _applicationContext.SetupGet(x => x.RealmId).Returns(realmId);

    Sender @default = new(new Email(_faker.Person.Email), SenderHelper.GenerateSendGridSettings(), isDefault: true, actorId, SenderId.NewId(realmId));
    _senderQuerier.Setup(x => x.FindDefaultIdAsync(SenderKind.Email, _cancellationToken)).ReturnsAsync(@default.Id);
    _senderRepository.Setup(x => x.LoadAsync(@default.Id, _cancellationToken)).ReturnsAsync(@default);

    Sender sender = new(new Email(_faker.Internet.Email()), SenderHelper.GenerateSendGridSettings(), isDefault: false, actorId, SenderId.NewId(realmId));
    _senderRepository.Setup(x => x.LoadAsync(sender.Id, _cancellationToken)).ReturnsAsync(sender);

    SenderDto dto = new();
    _senderQuerier.Setup(x => x.ReadAsync(sender, _cancellationToken)).ReturnsAsync(dto);

    SetDefaultSender command = new(sender.EntityId);
    SenderDto? result = await _handler.HandleAsync(command, _cancellationToken);
    Assert.NotNull(result);
    Assert.Same(dto, result);

    Assert.False(@default.IsDefault);
    Assert.Contains(@default.Changes, change => change is SenderSetDefault setDefault && !setDefault.IsDefault && setDefault.ActorId == actorId);

    Assert.True(sender.IsDefault);
    Assert.Contains(sender.Changes, change => change is SenderSetDefault setDefault && setDefault.IsDefault && setDefault.ActorId == actorId);

    _senderRepository.Verify(x => x.SaveAsync(
      It.Is<IEnumerable<Sender>>(y => y.SequenceEqual(new Sender[] { @default, sender })),
      _cancellationToken), Times.Once);
  }
}
