using Krakenar.Contracts;
using Moq;
using RealmDto = Krakenar.Contracts.Realms.Realm;

namespace Krakenar.Core.Realms.Queries;

[Trait(Traits.Category, Categories.Unit)]
public class ReadRealmHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IRealmQuerier> _realmQuerier = new();

  private readonly ReadRealmHandler _handler;

  private readonly RealmDto _realm1 = new()
  {
    Id = Guid.NewGuid(),
    UniqueSlug = "old-realm"
  };
  private readonly RealmDto _realm2 = new()
  {
    Id = Guid.NewGuid(),
    UniqueSlug = "new-realm"
  };

  public ReadRealmHandlerTests()
  {
    _handler = new(_realmQuerier.Object);

    _realmQuerier.Setup(x => x.ReadAsync(_realm1.Id, _cancellationToken)).ReturnsAsync(_realm1);
    _realmQuerier.Setup(x => x.ReadAsync(_realm2.Id, _cancellationToken)).ReturnsAsync(_realm2);
    _realmQuerier.Setup(x => x.ReadAsync(_realm1.UniqueSlug, _cancellationToken)).ReturnsAsync(_realm1);
    _realmQuerier.Setup(x => x.ReadAsync(_realm2.UniqueSlug, _cancellationToken)).ReturnsAsync(_realm2);
  }

  [Fact(DisplayName = "It should return null when no realm was found.")]
  public async Task Given_NotFound_When_HandleAsync_Then_NullReturned()
  {
    ReadRealm query = new(Guid.Empty, "not-found");
    Assert.Null(await _handler.HandleAsync(query, _cancellationToken));
  }

  [Fact(DisplayName = "It should return the realm found by ID.")]
  public async Task Given_FoundById_When_HandleAsync_Then_RealmReturned()
  {
    ReadRealm query = new(_realm1.Id, "not-found");
    RealmDto? realm = await _handler.HandleAsync(query, _cancellationToken);

    Assert.NotNull(realm);
    Assert.Same(_realm1, realm);

    Assert.NotNull(query.UniqueSlug);
    _realmQuerier.Verify(x => x.ReadAsync(query.UniqueSlug, _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should return the realm found by unique slug.")]
  public async Task Given_FoundByUniqueSlug_When_HandleAsync_Then_RealmReturned()
  {
    ReadRealm query = new(Guid.Empty, _realm1.UniqueSlug);
    RealmDto? realm = await _handler.HandleAsync(query, _cancellationToken);

    Assert.NotNull(realm);
    Assert.Same(_realm1, realm);

    Assert.True(query.Id.HasValue);
    _realmQuerier.Verify(x => x.ReadAsync(query.Id.Value, _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should throw TooManyResultsException when multiple realms were found.")]
  public async Task Given_MultipleFound_When_HandleAsync_Then_TooManyResultsException()
  {
    ReadRealm query = new(_realm1.Id, _realm2.UniqueSlug);
    var exception = await Assert.ThrowsAsync<TooManyResultsException<RealmDto>>(async () => await _handler.HandleAsync(query, _cancellationToken));
    Assert.Equal(1, exception.ExpectedCount);
    Assert.Equal(2, exception.ActualCount);
  }
}
