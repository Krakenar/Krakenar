using Krakenar.Client.Contents;
using Krakenar.Contracts.Contents;

namespace Krakenar.Clients;

[Trait(Traits.Category, Categories.EndToEnd)]
public class ContentClientE2ETests : E2ETests
{
  private readonly CancellationToken _cancellationToken = default;

  public ContentClientE2ETests() : base()
  {
  }

  [Fact(DisplayName = "Contents should be managed correctly through the API client.")]
  public async Task Given_Realm_When_Client_Then_Success()
  {
    await InitializeRealmAsync(_cancellationToken);

    KrakenarSettings.Realm = Realm.UniqueSlug;
    using HttpClient contentTypeClient = new();
    ContentTypeClient contentTypes = new(contentTypeClient, KrakenarSettings);
    using HttpClient contentClient = new();
    ContentClient contents = new(contentClient, KrakenarSettings);

    Guid contentTypeId = Guid.Parse("82b2cff6-4fb1-4a48-8f9c-faaeba44f106");
    ContentType? contentType = await contentTypes.ReadAsync(contentTypeId, uniqueName: null, _cancellationToken);
    if (contentType is null)
    {
      CreateOrReplaceContentTypePayload contentTypePayload = new("Item");
      CreateOrReplaceContentTypeResult contentTypeResult = await contentTypes.CreateOrReplaceAsync(contentTypePayload, contentTypeId, version: null, _cancellationToken);
      Assert.True(contentTypeResult.Created);
      Assert.NotNull(contentTypeResult.ContentType);
      contentType = contentTypeResult.ContentType;
    }

    Guid contentId = Guid.NewGuid();
    CreateContentPayload createPayload = new(contentId.ToString(), contentTypeId.ToString())
    {
      Id = contentId,
      Language = "en"
    };
    Content content = await contents.CreateAsync(createPayload, _cancellationToken);
    Assert.Equal(contentId, content.Id);
    Assert.Equal(contentTypeId, content.ContentType.Id);
    Assert.Equal(createPayload.UniqueName, content.Invariant.UniqueName);
    ContentLocale locale = Assert.Single(content.Locales);
    Assert.Equal(createPayload.Language, locale.Language?.Locale.Code);
    Assert.Equal(createPayload.UniqueName, locale.UniqueName);

    SaveContentLocalePayload saveLocalePayload = new()
    {
      UniqueName = string.Join('_', content.Invariant.UniqueName, "bag-of-holding"),
      DisplayName = " Bag of Holding ",
      Description = "  This bag has an interior space considerably larger than its outside dimensions, roughly 2 feet in diameter at the mouth and 4 feet deep. The bag can hold up to 500 pounds, not exceeding a volume of 64 cubic feet. The bag weighs 15 pounds, regardless of its contents. Retrieving an item from the bag requires an action.\n\nIf the bag is overloaded, pierced, or torn, it ruptures and is destroyed, and its contents are scattered in the Astral Plane. If the bag is turned inside out, its contents spill forth, unharmed, but the bag must be put right before it can be used again. Breathing creatures inside the bag can survive up to a number of minutes equal to 10 divided by the number of creatures (minimum 1 minute), after which time they begin to suffocate.\n\nPlacing a _bag of holding_ inside an extradimensional space created by a [handy haversack](https://www.dndbeyond.com/magic-items/4650-handy-haversack), [portable hole](https://www.dndbeyond.com/magic-items/4699-portable-hole), or similar item instantly destroys both items and opens a gate to the Astral Plane. The gate originates where the one item was placed inside the other. Any creature within 10 feet of the gate is sucked through it to a random location on the Astral Plane. The gate then closes. The gate is one-way only and can’t be reopened.  "
    };
    content = (await contents.SaveLocaleAsync(contentId, saveLocalePayload, createPayload.Language, _cancellationToken))!;
    Assert.NotNull(content);
    Assert.Equal(contentId, content.Id);
    Assert.Equal(createPayload.UniqueName, content.Invariant.UniqueName);
    Assert.Null(content.Invariant.DisplayName);
    Assert.Null(content.Invariant.Description);
    locale = Assert.Single(content.Locales);
    Assert.Equal(saveLocalePayload.UniqueName, locale.UniqueName);
    Assert.Equal(saveLocalePayload.DisplayName.Trim(), locale.DisplayName);
    Assert.Equal(saveLocalePayload.Description.Trim(), locale.Description);
  }
}
