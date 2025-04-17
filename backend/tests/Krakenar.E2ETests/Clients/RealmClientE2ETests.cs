using Krakenar.Client.Realms;
using Krakenar.Contracts;
using Krakenar.Contracts.Realms;
using Krakenar.Contracts.Search;

namespace Krakenar.Clients;

[Trait(Traits.Category, Categories.EndToEnd)]
public class RealmClientE2ETests : E2ETests
{
  private readonly CancellationToken _cancellationToken = default;

  public RealmClientE2ETests() : base()
  {
  }

  [Fact(DisplayName = "Realms should be managed correctly through the API client.")]
  public async Task Given_Realm_When_Client_Then_Success()
  {
    await InitializeRealmAsync(_cancellationToken);

    using HttpClient httpClient = new();
    RealmClient realms = new(httpClient, KrakenarSettings);

    Realm? realm = await realms.ReadAsync(Realm.Id, uniqueSlug: null, _cancellationToken);
    Assert.NotNull(realm);
    Assert.Equal(Realm.Id, realm.Id);

    realm = await realms.ReadAsync(id: null, realm.UniqueSlug, _cancellationToken);
    Assert.NotNull(realm);
    Assert.Equal(Realm.Id, realm.Id);

    UpdateRealmPayload updateRealm = new()
    {
      Description = new Change<string>("  This is the realm for End-to-End (E2E) tests.  "),
      CustomAttributes = [new CustomAttribute("key", "value")]
    };
    realm = await realms.UpdateAsync(Realm.Id, updateRealm, _cancellationToken);
    Assert.NotNull(realm);
    Assert.Equal(Realm.Id, realm.Id);

    SearchRealmsPayload searchRealms = new()
    {
      Ids = [realm.Id],
      Search = new TextSearch([new SearchTerm("%e2e%")])
    };
    SearchResults<Realm> results = await realms.SearchAsync(searchRealms, _cancellationToken);
    Assert.Equal(1, results.Total);
    realm = Assert.Single(results.Items);
    Assert.Equal(Realm.Id, realm.Id);
  }
}
