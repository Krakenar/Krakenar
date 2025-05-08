using Moq;
using MessageDto = Krakenar.Contracts.Messages.Message;

namespace Krakenar.Core.Messages.Queries;

[Trait(Traits.Category, Categories.Unit)]
public class ReadMessageHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IMessageQuerier> _messageQuerier = new();

  private readonly ReadMessageHandler _handler;

  private readonly MessageDto _message = new()
  {
    Id = Guid.NewGuid()
  };

  public ReadMessageHandlerTests()
  {
    _handler = new(_messageQuerier.Object);

    _messageQuerier.Setup(x => x.ReadAsync(_message.Id, _cancellationToken)).ReturnsAsync(_message);
  }

  [Fact(DisplayName = "It should return null when the message was not found.")]
  public async Task Given_NotFound_When_HandleAsync_Then_NullReturned()
  {
    ReadMessage query = new(Guid.Empty);
    Assert.Null(await _handler.HandleAsync(query, _cancellationToken));
  }

  [Fact(DisplayName = "It should return the message found by ID.")]
  public async Task Given_FoundById_When_HandleAsync_Then_MessageReturned()
  {
    ReadMessage query = new(_message.Id);
    MessageDto? message = await _handler.HandleAsync(query, _cancellationToken);

    Assert.NotNull(message);
    Assert.Same(_message, message);
  }
}
