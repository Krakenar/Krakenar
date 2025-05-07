using Bogus;
using Krakenar.Client.Senders;
using Krakenar.Contracts;
using Krakenar.Contracts.Search;
using Krakenar.Contracts.Senders;
using Krakenar.Contracts.Senders.Settings;
using Krakenar.Contracts.Users;

namespace Krakenar.Clients;

[Trait(Traits.Category, Categories.EndToEnd)]
public class SenderClientE2ETests : E2ETests
{
  private readonly CancellationToken _cancellationToken = default;
  private readonly Faker _faker = new();

  public SenderClientE2ETests() : base()
  {
  }

  [Fact(DisplayName = "Senders should be managed correctly through the API client.")]
  public async Task Given_Realm_When_Client_Then_Success()
  {
    await InitializeRealmAsync(_cancellationToken);

    using HttpClient httpClient = new();
    KrakenarSettings.Realm = Realm.UniqueSlug;
    SenderClient senders = new(httpClient, KrakenarSettings);

    Guid id = Guid.Parse("8c0bd581-6b87-431b-9144-a8e39fc61c63");
    Sender? sender = await senders.ReadAsync(id, kind: null, _cancellationToken);

    CreateOrReplaceSenderPayload createOrReplaceSender = new()
    {
      Email = new EmailPayload(_faker.Internet.Email()),
      SendGrid = new SendGridSettings(SenderHelper.GenerateSendGridSettings())
    };
    CreateOrReplaceSenderResult senderResult = await senders.CreateOrReplaceAsync(createOrReplaceSender, id, version: null, _cancellationToken);
    Assert.Equal(senderResult.Created, sender is null);
    sender = senderResult.Sender;
    Assert.NotNull(sender);
    Assert.Equal(id, sender.Id);
    Assert.Equal(createOrReplaceSender.Email.Address, sender.Email?.Address);
    Assert.Equal(createOrReplaceSender.SendGrid, sender.SendGrid);

    UpdateSenderPayload updateSender = new()
    {
      DisplayName = new Change<string>($" {_faker.Company.CompanyName()} "),
      Description = new Change<string>($"  This is the default SendGrid sender.  ")
    };
    sender = await senders.UpdateAsync(id, updateSender, _cancellationToken);
    Assert.NotNull(sender);
    Assert.Equal(createOrReplaceSender.Email.Address, sender.Email?.Address);
    Assert.Equal(updateSender.DisplayName.Value?.Trim(), sender.DisplayName);
    Assert.Equal(updateSender.Description.Value?.Trim(), sender.Description);
    Assert.Equal(createOrReplaceSender.SendGrid, sender.SendGrid);

    sender = await senders.ReadAsync(id: null, SenderKind.Email, _cancellationToken);
    Assert.NotNull(sender);
    Assert.Equal(id, sender.Id);

    SearchSendersPayload searchSenders = new()
    {
      Kind = SenderKind.Email,
      Provider = SenderProvider.SendGrid,
      Ids = [sender.Id]
    };
    SearchResults<Sender> results = await senders.SearchAsync(searchSenders, _cancellationToken);
    Assert.Equal(1, results.Total);
    sender = Assert.Single(results.Items);
    Assert.Equal(id, sender.Id);

    createOrReplaceSender = new CreateOrReplaceSenderPayload
    {
      Email = new EmailPayload(_faker.Internet.Email()),
      SendGrid = new SendGridSettings(SenderHelper.GenerateSendGridSettings())
    };
    senderResult = await senders.CreateOrReplaceAsync(createOrReplaceSender, id: null, version: null, _cancellationToken);
    Assert.True(senderResult.Created);
    sender = senderResult.Sender;
    Assert.NotNull(sender);
    Guid otherId = sender.Id;

    sender = await senders.SetDefaultAsync(sender.Id, _cancellationToken);
    Assert.NotNull(sender);
    Assert.True(sender.IsDefault);

    sender = await senders.SetDefaultAsync(id, _cancellationToken);
    Assert.NotNull(sender);
    Assert.Equal(id, sender.Id);
    Assert.True(sender.IsDefault);

    sender = await senders.DeleteAsync(otherId, _cancellationToken);
    Assert.NotNull(sender);
    Assert.Equal(otherId, sender.Id);
  }
}
