using Krakenar.Contracts.Search;
using Krakenar.Contracts.Templates;
using Moq;
using TemplateDto = Krakenar.Contracts.Templates.Template;

namespace Krakenar.Core.Templates.Queries;

[Trait(Traits.Category, Categories.Unit)]
public class SearchTemplatesHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<ITemplateQuerier> _templateQuerier = new();

  private readonly SearchTemplatesHandler _handler;

  public SearchTemplatesHandlerTests()
  {
    _handler = new(_templateQuerier.Object);
  }

  [Fact(DisplayName = "It should search templates.")]
  public async Task Given_Payload_When_HandleAsync_Then_Searched()
  {
    SearchTemplatesPayload payload = new();
    SearchResults<TemplateDto> expected = new();
    _templateQuerier.Setup(x => x.SearchAsync(payload, _cancellationToken)).ReturnsAsync(expected);

    SearchTemplates query = new(payload);
    SearchResults<TemplateDto> results = await _handler.HandleAsync(query, _cancellationToken);
    Assert.Same(expected, results);
  }
}
