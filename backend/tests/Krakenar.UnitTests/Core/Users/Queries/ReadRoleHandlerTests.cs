using Bogus;
using Krakenar.Contracts.Users;
using Moq;
using CustomIdentifierDto = Krakenar.Contracts.CustomIdentifier;
using EmailDto = Krakenar.Contracts.Users.Email;
using UserDto = Krakenar.Contracts.Users.User;

namespace Krakenar.Core.Users.Queries;

[Trait(Traits.Category, Categories.Unit)]
public class ReadUserHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;
  private readonly Faker _faker = new();

  private readonly Mock<IApplicationContext> _applicationContext = new();
  private readonly Mock<IUserQuerier> _userQuerier = new();

  private readonly ReadUserHandler _handler;

  private readonly UserDto _user1;
  private readonly UserDto _user2;
  private readonly UserDto _user3;

  public ReadUserHandlerTests()
  {
    _handler = new(_applicationContext.Object, _userQuerier.Object);

    _user1 = new()
    {
      Id = Guid.NewGuid(),
      UniqueName = _faker.Person.UserName,
      Email = new EmailDto(_faker.Person.Email)
    };
    _user1.CustomIdentifiers.Add(new CustomIdentifierDto("Google", "5424946770"));
    _userQuerier.Setup(x => x.ReadAsync(_user1.Id, _cancellationToken)).ReturnsAsync(_user1);
    _userQuerier.Setup(x => x.ReadAsync(_user1.UniqueName, _cancellationToken)).ReturnsAsync(_user1);
    _userQuerier.Setup(x => x.ReadAsync(It.Is<IEmail>(e => e.Address == _user1.Email.Address), _cancellationToken)).ReturnsAsync([_user1]);
    _userQuerier.Setup(x => x.ReadAsync(_user1.CustomIdentifiers.Single(), _cancellationToken)).ReturnsAsync(_user1);

    _user2 = new()
    {
      Id = Guid.NewGuid(),
      UniqueName = _faker.Internet.UserName(),
      Email = new EmailDto(_faker.Internet.Email())
    };
    _user2.CustomIdentifiers.Add(new CustomIdentifierDto("Google", "8395440438"));
    _userQuerier.Setup(x => x.ReadAsync(_user2.Id, _cancellationToken)).ReturnsAsync(_user2);
    _userQuerier.Setup(x => x.ReadAsync(_user2.UniqueName, _cancellationToken)).ReturnsAsync(_user2);
    _userQuerier.Setup(x => x.ReadAsync(It.Is<IEmail>(e => e.Address == _user2.Email.Address), _cancellationToken)).ReturnsAsync([_user2]);
    _userQuerier.Setup(x => x.ReadAsync(_user2.CustomIdentifiers.Single(), _cancellationToken)).ReturnsAsync(_user2);

    _user3 = new()
    {
      Id = Guid.NewGuid(),
      UniqueName = _faker.Internet.UserName(),
      Email = new EmailDto(_faker.Internet.Email())
    };
    _user3.CustomIdentifiers.Add(new CustomIdentifierDto("Google", "6935809648"));
    _userQuerier.Setup(x => x.ReadAsync(_user3.Id, _cancellationToken)).ReturnsAsync(_user3);
    _userQuerier.Setup(x => x.ReadAsync(_user3.UniqueName, _cancellationToken)).ReturnsAsync(_user3);
    _userQuerier.Setup(x => x.ReadAsync(It.Is<IEmail>(e => e.Address == _user3.Email.Address), _cancellationToken)).ReturnsAsync([_user3]);
    _userQuerier.Setup(x => x.ReadAsync(_user3.CustomIdentifiers.Single(), _cancellationToken)).ReturnsAsync(_user3);
  }

  [Fact(DisplayName = "It should return null when no user was found.")]
  public async Task Given_NotFound_When_HandleAsync_Then_NullReturned()
  {
    ReadUser query = new(Guid.Empty, "not_found", new CustomIdentifierDto("Google", "1234567890"));
    Assert.Null(await _handler.HandleAsync(query, _cancellationToken));
  }

  [Fact(DisplayName = "It should return the user found by ID.")]
  public async Task Given_FoundById_When_HandleAsync_Then_UserReturned()
  {
    CustomIdentifierDto customIdentifier = new("Google", "1234567890");

    ReadUser query = new(_user1.Id, "not_found", customIdentifier);
    UserDto? user = await _handler.HandleAsync(query, _cancellationToken);

    Assert.NotNull(user);
    Assert.Same(_user1, user);

    Assert.NotNull(query.UniqueName);
    _userQuerier.Verify(x => x.ReadAsync(query.UniqueName, _cancellationToken), Times.Once);
    _userQuerier.Verify(x => x.ReadAsync(customIdentifier, _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should return the user found by custom identifier.")]
  public async Task Given_FoundByCustomIdentifier_When_HandleAsync_Then_UserReturned()
  {
    ReadUser query = new(Guid.Empty, "not_found", _user3.CustomIdentifiers.Single());
    UserDto? user = await _handler.HandleAsync(query, _cancellationToken);

    Assert.NotNull(user);
    Assert.Same(_user3, user);

    Assert.True(query.Id.HasValue);
    Assert.NotNull(query.UniqueName);
    _userQuerier.Verify(x => x.ReadAsync(query.Id.Value, _cancellationToken), Times.Once);
    _userQuerier.Verify(x => x.ReadAsync(query.UniqueName, _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should return the user found by email address.")]
  public async Task Given_FoundByEmailAddress_When_HandleAsync_Then_UserReturned()
  {
    _applicationContext.SetupGet(x => x.RequireUniqueEmail).Returns(true);

    CustomIdentifierDto customIdentifier = new("Google", "1234567890");

    Assert.NotNull(_user1.Email);
    ReadUser query = new(Guid.Empty, _user1.Email.Address, customIdentifier);
    UserDto? user = await _handler.HandleAsync(query, _cancellationToken);

    Assert.NotNull(user);
    Assert.Same(_user1, user);

    Assert.True(query.Id.HasValue);
    Assert.NotNull(query.UniqueName);
    _userQuerier.Verify(x => x.ReadAsync(query.Id.Value, _cancellationToken), Times.Once);
    _userQuerier.Verify(x => x.ReadAsync(query.UniqueName, _cancellationToken), Times.Once);
    _userQuerier.Verify(x => x.ReadAsync(customIdentifier, _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should return the user found by unique name.")]
  public async Task Given_FoundByUniqueName_When_HandleAsync_Then_UserReturned()
  {
    CustomIdentifierDto customIdentifier = new("Google", "1234567890");

    ReadUser query = new(Guid.Empty, _user1.UniqueName, customIdentifier);
    UserDto? user = await _handler.HandleAsync(query, _cancellationToken);

    Assert.NotNull(user);
    Assert.Same(_user1, user);

    Assert.True(query.Id.HasValue);
    _userQuerier.Verify(x => x.ReadAsync(query.Id.Value, _cancellationToken), Times.Once);
    _userQuerier.Verify(x => x.ReadAsync(It.IsAny<IEmail>(), It.IsAny<CancellationToken>()), Times.Never);
    _userQuerier.Verify(x => x.ReadAsync(customIdentifier, _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should throw TooManyResultsException when multiple users were found.")]
  public async Task Given_MultipleFound_When_HandleAsync_Then_TooManyResultsException()
  {
    ReadUser query = new(_user1.Id, _user2.UniqueName, _user3.CustomIdentifiers.Single());
    var exception = await Assert.ThrowsAsync<TooManyResultsException<UserDto>>(async () => await _handler.HandleAsync(query, _cancellationToken));
    Assert.Equal(1, exception.ExpectedCount);
    Assert.Equal(3, exception.ActualCount);
  }
}
