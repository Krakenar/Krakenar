using Moq;
using OneTimePasswordDto = Krakenar.Contracts.Passwords.OneTimePassword;

namespace Krakenar.Core.Passwords.Queries;

[Trait(Traits.Category, Categories.Unit)]
public class ReadOneTimePasswordHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IOneTimePasswordQuerier> _oneTimePasswordQuerier = new();

  private readonly ReadOneTimePasswordHandler _handler;

  public ReadOneTimePasswordHandlerTests()
  {
    _handler = new(_oneTimePasswordQuerier.Object);
  }

  [Fact(DisplayName = "It should return null when the One-Time Password (OTP) could not be found.")]
  public async Task Given_NotFound_When_HandleAsync_Then_NullReturned()
  {
    Assert.Null(await _handler.HandleAsync(new ReadOneTimePassword(Guid.NewGuid()), _cancellationToken));
  }

  [Fact(DisplayName = "It should return the One-Time Password (OTP) found by ID.")]
  public async Task Given_Found_When_HandleAsync_Then_OneTimePasswordReturned()
  {
    OneTimePasswordDto oneTimePassword = new()
    {
      Id = Guid.NewGuid()
    };
    _oneTimePasswordQuerier.Setup(x => x.ReadAsync(oneTimePassword.Id, _cancellationToken)).ReturnsAsync(oneTimePassword);

    ReadOneTimePassword query = new(oneTimePassword.Id);
    OneTimePasswordDto? result = await _handler.HandleAsync(query, _cancellationToken);

    Assert.NotNull(result);
    Assert.Same(oneTimePassword, result);
  }
}
