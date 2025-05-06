using Krakenar.Contracts;
using Moq;
using TemplateDto = Krakenar.Contracts.Templates.Template;

namespace Krakenar.Core.Templates.Queries;

[Trait(Traits.Category, Categories.Unit)]
public class ReadTemplateHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<ITemplateQuerier> _templateQuerier = new();

  private readonly ReadTemplateHandler _handler;

  private readonly TemplateDto _accountActivation = new()
  {
    Id = Guid.NewGuid(),
    UniqueName = "AccountActivation"
  };
  private readonly TemplateDto _passwordRecovery = new()
  {
    Id = Guid.NewGuid(),
    UniqueName = "PasswordRecovery"
  };

  public ReadTemplateHandlerTests()
  {
    _handler = new(_templateQuerier.Object);

    _templateQuerier.Setup(x => x.ReadAsync(_accountActivation.Id, _cancellationToken)).ReturnsAsync(_accountActivation);
    _templateQuerier.Setup(x => x.ReadAsync(_passwordRecovery.Id, _cancellationToken)).ReturnsAsync(_passwordRecovery);
    _templateQuerier.Setup(x => x.ReadAsync(_accountActivation.UniqueName, _cancellationToken)).ReturnsAsync(_accountActivation);
    _templateQuerier.Setup(x => x.ReadAsync(_passwordRecovery.UniqueName, _cancellationToken)).ReturnsAsync(_passwordRecovery);
  }

  [Fact(DisplayName = "It should return null when no template was found.")]
  public async Task Given_NotFound_When_HandleAsync_Then_NullReturned()
  {
    ReadTemplate query = new(Guid.Empty, "not_found");
    Assert.Null(await _handler.HandleAsync(query, _cancellationToken));
  }

  [Fact(DisplayName = "It should return the template found by ID.")]
  public async Task Given_FoundById_When_HandleAsync_Then_TemplateReturned()
  {
    ReadTemplate query = new(_accountActivation.Id, "not_found");
    TemplateDto? template = await _handler.HandleAsync(query, _cancellationToken);

    Assert.NotNull(template);
    Assert.Same(_accountActivation, template);

    Assert.NotNull(query.UniqueName);
    _templateQuerier.Verify(x => x.ReadAsync(query.UniqueName, _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should return the template found by unique name.")]
  public async Task Given_FoundByUniqueName_When_HandleAsync_Then_TemplateReturned()
  {
    ReadTemplate query = new(Guid.Empty, _accountActivation.UniqueName);
    TemplateDto? template = await _handler.HandleAsync(query, _cancellationToken);

    Assert.NotNull(template);
    Assert.Same(_accountActivation, template);

    Assert.True(query.Id.HasValue);
    _templateQuerier.Verify(x => x.ReadAsync(query.Id.Value, _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should throw TooManyResultsException when multiple templates were found.")]
  public async Task Given_MultipleFound_When_HandleAsync_Then_TooManyResultsException()
  {
    ReadTemplate query = new(_accountActivation.Id, _passwordRecovery.UniqueName);
    var exception = await Assert.ThrowsAsync<TooManyResultsException<TemplateDto>>(async () => await _handler.HandleAsync(query, _cancellationToken));
    Assert.Equal(1, exception.ExpectedCount);
    Assert.Equal(2, exception.ActualCount);
  }
}
