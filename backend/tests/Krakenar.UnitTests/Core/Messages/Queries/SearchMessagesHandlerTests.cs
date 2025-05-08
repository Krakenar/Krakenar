using Krakenar.Contracts.Messages;
using Krakenar.Contracts.Search;
using Moq;
using MessageDto = Krakenar.Contracts.Messages.Message;

namespace Krakenar.Core.Messages.Queries;

[Trait(Traits.Category, Categories.Unit)]
public class SearchMessagesHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IMessageQuerier> _messageQuerier = new();

  private readonly SearchMessagesHandler _handler;

  public SearchMessagesHandlerTests()
  {
    _handler = new(_messageQuerier.Object);
  }

  [Fact(DisplayName = "It should search messages.")]
  public async Task Given_Payload_When_HandleAsync_Then_Searched()
  {
    SearchMessagesPayload payload = new();
    SearchResults<MessageDto> expected = new();
    _messageQuerier.Setup(x => x.SearchAsync(payload, _cancellationToken)).ReturnsAsync(expected);

    SearchMessages query = new(payload);
    SearchResults<MessageDto> results = await _handler.HandleAsync(query, _cancellationToken);
    Assert.Same(expected, results);
  }
}
