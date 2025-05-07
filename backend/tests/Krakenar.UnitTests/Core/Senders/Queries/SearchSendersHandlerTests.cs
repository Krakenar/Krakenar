using Krakenar.Contracts.Search;
using Krakenar.Contracts.Senders;
using Moq;
using SenderDto = Krakenar.Contracts.Senders.Sender;

namespace Krakenar.Core.Senders.Queries;

[Trait(Traits.Category, Categories.Unit)]
public class SearchSendersHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<ISenderQuerier> _senderQuerier = new();

  private readonly SearchSendersHandler _handler;

  public SearchSendersHandlerTests()
  {
    _handler = new(_senderQuerier.Object);
  }

  [Fact(DisplayName = "It should search senders.")]
  public async Task Given_Payload_When_HandleAsync_Then_Searched()
  {
    SearchSendersPayload payload = new();
    SearchResults<SenderDto> expected = new();
    _senderQuerier.Setup(x => x.SearchAsync(payload, _cancellationToken)).ReturnsAsync(expected);

    SearchSenders query = new(payload);
    SearchResults<SenderDto> results = await _handler.HandleAsync(query, _cancellationToken);
    Assert.Same(expected, results);
  }
}
