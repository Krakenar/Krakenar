using Krakenar.Core.Tokens;
using Logitar.Security.Cryptography;
using Moq;

namespace Krakenar.Core.Realms;

[Trait(Traits.Category, Categories.Unit)]
public class RealmServiceTests
{
  private readonly CancellationToken _cancellationToken = default;
  private readonly Secret _secret = new(RandomStringGenerator.GetString(Secret.MinimumLength));

  private readonly Mock<IRealmQuerier> _realmQuerier = new();
  private readonly Mock<IRealmRepository> _realmRepository = new();

  private readonly RealmService _service;

  public RealmServiceTests()
  {
    _service = new(_realmQuerier.Object, _realmRepository.Object);
  }

  [Fact(DisplayName = "SaveAsync: it should save the realm when the unique slug has not changed.")]
  public async Task Given_UniqueSlugNotChanged_When_SaveAsync_Then_Saved()
  {
    Realm realm = new(new Slug("new-world"), _secret);
    realm.ClearChanges();

    realm.Delete();
    await _service.SaveAsync(realm, _cancellationToken);

    _realmQuerier.Verify(x => x.FindIdAsync(It.IsAny<Slug>(), It.IsAny<CancellationToken>()), Times.Never);
    _realmRepository.Verify(x => x.SaveAsync(realm, _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "SaveAsync: it should save the realm when there is no unique slug conflict.")]
  public async Task Given_NoUniqueSlugConflict_When_SaveAsync_Then_Saved()
  {
    Realm realm = new(new Slug("old-world"), _secret);
    _realmQuerier.Setup(x => x.FindIdAsync(realm.UniqueSlug, _cancellationToken)).ReturnsAsync(realm.Id);

    await _service.SaveAsync(realm, _cancellationToken);

    _realmQuerier.Verify(x => x.FindIdAsync(realm.UniqueSlug, _cancellationToken), Times.Once);
    _realmRepository.Verify(x => x.SaveAsync(realm, _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "SaveAsync: it should throw UniqueSlugAlreadyUsedException when there is a unique slug conflict.")]
  public async Task Given_UniqueSlugConflict_When_SaveAsync_Then_UniqueSlugAlreadyUsedException()
  {
    Realm conflict = new(new Slug("the-world"), _secret);
    _realmQuerier.Setup(x => x.FindIdAsync(conflict.UniqueSlug, _cancellationToken)).ReturnsAsync(conflict.Id);

    Realm realm = new(new Slug("other-world"), _secret);
    realm.ClearChanges();
    realm.SetUniqueSlug(conflict.UniqueSlug);

    var exception = await Assert.ThrowsAsync<UniqueSlugAlreadyUsedException>(async () => await _service.SaveAsync(realm, _cancellationToken));
    Assert.Equal(realm.Id.ToGuid(), exception.RealmId);
    Assert.Equal(conflict.Id.ToGuid(), exception.ConflictId);
    Assert.Equal(realm.UniqueSlug.Value, exception.UniqueSlug);
    Assert.Equal("UniqueSlug", exception.PropertyName);
  }
}
