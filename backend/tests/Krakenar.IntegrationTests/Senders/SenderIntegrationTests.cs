using Krakenar.Contracts;
using Krakenar.Contracts.Search;
using Krakenar.Contracts.Senders;
using Krakenar.Contracts.Users;
using Krakenar.Core.Senders;
using Krakenar.Core.Senders.Settings;
using Logitar;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Email = Krakenar.Core.Users.Email;
using Phone = Krakenar.Core.Users.Phone;
using Sender = Krakenar.Core.Senders.Sender;
using SenderDto = Krakenar.Contracts.Senders.Sender;
using SendGridSettings = Krakenar.Core.Senders.Settings.SendGridSettings;
using SendGridSettingsDto = Krakenar.Contracts.Senders.Settings.SendGridSettings;
using TwilioSettingsDto = Krakenar.Contracts.Senders.Settings.TwilioSettings;

namespace Krakenar.Senders;

[Trait(Traits.Category, Categories.Integration)]
public class SenderIntegrationTests : IntegrationTests
{
  private readonly ISenderRepository _senderRepository;
  private readonly ISenderService _senderService;

  private readonly Sender _sendGrid;
  private readonly Sender _twilio;

  public SenderIntegrationTests() : base()
  {
    _senderRepository = ServiceProvider.GetRequiredService<ISenderRepository>();
    _senderService = ServiceProvider.GetRequiredService<ISenderService>();

    _sendGrid = new(new Email(Faker.Person.Email), SenderHelper.GenerateSendGridSettings(), isDefault: true, actorId: null, SenderId.NewId(Realm.Id));
    _twilio = new(new Phone("+15148454636", countryCode: "CA"), SenderHelper.GenerateTwilioSettings(), isDefault: true, actorId: null, SenderId.NewId(Realm.Id));
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    await _senderRepository.SaveAsync([_sendGrid, _twilio]);
  }

  [Fact(DisplayName = "It should create a new SendGrid sender.")]
  public async Task Given_SendGridNotExist_When_CreateOrReplace_Then_Created()
  {
    _sendGrid.SetDefault(isDefault: false, ActorId);
    await _senderRepository.SaveAsync(_sendGrid);

    CreateOrReplaceSenderPayload payload = new()
    {
      Email = new EmailPayload($" {Faker.Internet.Email()} "),
      DisplayName = $" {Faker.Company.CompanyName()} ",
      Description = "  This is the default SendGrid sender.  ",
      SendGrid = new SendGridSettingsDto(SenderHelper.GenerateSendGridSettings())
    };

    Guid id = Guid.NewGuid();
    CreateOrReplaceSenderResult result = await _senderService.CreateOrReplaceAsync(payload, id);
    Assert.True(result.Created);

    SenderDto? sender = result.Sender;
    Assert.NotNull(sender);
    Assert.Equal(id, sender.Id);
    Assert.Equal(3, sender.Version);
    Assert.Equal(Actor, sender.CreatedBy);
    Assert.Equal(DateTime.UtcNow, sender.CreatedOn.AsUniversalTime(), TimeSpan.FromSeconds(10));
    Assert.Equal(Actor, sender.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, sender.UpdatedOn.AsUniversalTime(), TimeSpan.FromSeconds(10));

    Assert.Equal(RealmDto, sender.Realm);
    Assert.Equal(SenderKind.Email, sender.Kind);
    Assert.True(sender.IsDefault);
    Assert.NotNull(sender.Email);
    Assert.Equal(payload.Email.Address.Trim(), sender.Email.Address);
    Assert.Null(sender.Phone);
    Assert.Equal(payload.DisplayName.Trim(), sender.DisplayName);
    Assert.Equal(payload.Description.Trim(), sender.Description);
    Assert.Equal(SenderProvider.SendGrid, sender.Provider);
    Assert.Equal(payload.SendGrid, sender.SendGrid);
    Assert.Null(sender.Twilio);
  }

  [Fact(DisplayName = "It should create a new Twilio sender.")]
  public async Task Given_TwilioNotExist_When_CreateOrReplace_Then_Created()
  {
    CreateOrReplaceSenderPayload payload = new()
    {
      Phone = new PhonePayload("+15148422112", countryCode: "CA"),
      DisplayName = $" {Faker.Company.CompanyName()} ",
      Description = "  This is not the default Twilio sender.  ",
      Twilio = new TwilioSettingsDto(SenderHelper.GenerateTwilioSettings())
    };

    CreateOrReplaceSenderResult result = await _senderService.CreateOrReplaceAsync(payload);
    Assert.True(result.Created);

    SenderDto? sender = result.Sender;
    Assert.NotNull(sender);
    Assert.NotEqual(Guid.Empty, sender.Id);
    Assert.Equal(3, sender.Version);
    Assert.Equal(Actor, sender.CreatedBy);
    Assert.Equal(DateTime.UtcNow, sender.CreatedOn.AsUniversalTime(), TimeSpan.FromSeconds(10));
    Assert.Equal(Actor, sender.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, sender.UpdatedOn.AsUniversalTime(), TimeSpan.FromSeconds(10));

    Assert.Equal(RealmDto, sender.Realm);
    Assert.Equal(SenderKind.Phone, sender.Kind);
    Assert.False(sender.IsDefault);
    Assert.Null(sender.Email);
    Assert.NotNull(sender.Phone);
    Assert.Equal(payload.Phone.Number, sender.Phone.Number);
    Assert.Equal(payload.DisplayName.Trim(), sender.DisplayName);
    Assert.Equal(payload.Description.Trim(), sender.Description);
    Assert.Equal(SenderProvider.Twilio, sender.Provider);
    Assert.Null(sender.SendGrid);
    Assert.Equal(payload.Twilio, sender.Twilio);
  }

  [Fact(DisplayName = "It should delete the sender.")]
  public async Task Given_Sender_When_Delete_Then_Deleted()
  {
    SenderDto? sender = await _senderService.DeleteAsync(_sendGrid.EntityId);
    Assert.NotNull(sender);
    Assert.Equal(_sendGrid.EntityId, sender.Id);

    Assert.Empty(await KrakenarContext.Senders.AsNoTracking().Where(x => x.StreamId == _sendGrid.Id.Value).ToArrayAsync());
  }

  [Fact(DisplayName = "It should read the sender by ID.")]
  public async Task Given_Id_When_Read_Then_Found()
  {
    SenderDto? sender = await _senderService.ReadAsync(_sendGrid.EntityId);
    Assert.NotNull(sender);
    Assert.Equal(_sendGrid.EntityId, sender.Id);
  }

  [Fact(DisplayName = "It should read the default email sender.")]
  public async Task Given_EmailSender_When_Read_Then_Found()
  {
    SenderDto? sender = await _senderService.ReadAsync(id: null, SenderKind.Email);
    Assert.NotNull(sender);
    Assert.Equal(_sendGrid.EntityId, sender.Id);
  }

  [Fact(DisplayName = "It should read the default phone sender.")]
  public async Task Given_PhoneSender_When_Read_Then_Found()
  {
    SenderDto? sender = await _senderService.ReadAsync(id: null, SenderKind.Phone);
    Assert.NotNull(sender);
    Assert.Equal(_twilio.EntityId, sender.Id);
  }

  [Fact(DisplayName = "It should replace an existing sender.")]
  public async Task Given_NoVersion_When_CreateOrReplace_Then_Replaced()
  {
    CreateOrReplaceSenderPayload payload = new()
    {
      Email = new EmailPayload($" {Faker.Internet.Email()} "),
      DisplayName = $" {Faker.Company.CompanyName()} ",
      Description = "  This is the default SendGrid settings.  ",
      SendGrid = new SendGridSettingsDto(SenderHelper.GenerateSendGridSettings())
    };

    CreateOrReplaceSenderResult result = await _senderService.CreateOrReplaceAsync(payload, _sendGrid.EntityId);
    Assert.False(result.Created);

    SenderDto? sender = result.Sender;
    Assert.NotNull(sender);
    Assert.Equal(_sendGrid.EntityId, sender.Id);
    Assert.Equal(_sendGrid.Version + 2, sender.Version);
    Assert.Equal(Actor, sender.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, sender.UpdatedOn.AsUniversalTime(), TimeSpan.FromSeconds(10));

    Assert.Equal(payload.Email.Address.Trim(), sender.Email?.Address);
    Assert.Equal(payload.DisplayName.Trim(), sender.DisplayName);
    Assert.Equal(payload.Description.Trim(), sender.Description);
    Assert.Equal(payload.SendGrid, sender.SendGrid);
  }

  [Fact(DisplayName = "It should replace an existing sender from reference.")]
  public async Task Given_Version_When_CreateOrReplace_Then_Replaced()
  {
    TwilioSettings oldSettings = (TwilioSettings)_twilio.Settings;
    long version = _twilio.Version;

    TwilioSettings twilioSettings = SenderHelper.GenerateTwilioSettings();
    _twilio.SetSettings(twilioSettings, ActorId);
    await _senderRepository.SaveAsync(_twilio);

    CreateOrReplaceSenderPayload payload = new()
    {
      Phone = new PhonePayload("+15148422112", countryCode: "CA"),
      DisplayName = $" {Faker.Company.CompanyName()} ",
      Description = "  This is the default Twilio settings.  ",
      Twilio = new TwilioSettingsDto(oldSettings)
    };

    CreateOrReplaceSenderResult result = await _senderService.CreateOrReplaceAsync(payload, _twilio.EntityId, version);
    Assert.False(result.Created);

    SenderDto? sender = result.Sender;
    Assert.NotNull(sender);
    Assert.Equal(_twilio.EntityId, sender.Id);
    Assert.Equal(_twilio.Version + 1, sender.Version);
    Assert.Equal(Actor, sender.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, sender.UpdatedOn.AsUniversalTime(), TimeSpan.FromSeconds(10));

    Assert.Equal(payload.Phone.Number, sender.Phone?.Number);
    Assert.Equal(payload.DisplayName.Trim(), sender.DisplayName);
    Assert.Equal(payload.Description.Trim(), sender.Description);
    Assert.Equal(new TwilioSettingsDto(twilioSettings), sender.Twilio);
  }

  [Fact(DisplayName = "It should return null when the sender cannot be found.")]
  public async Task Given_NotFound_When_Read_Then_NullReturned()
  {
    Assert.Null(await _senderService.ReadAsync(Guid.Empty, kind: null));
  }

  [Fact(DisplayName = "It should return the correct search results.")]
  public async Task Given_Senders_When_Search_Then_CorrectResults()
  {
    Sender sendGrid = new(new Email(Faker.Internet.Email()), SenderHelper.GenerateSendGridSettings(), isDefault: false, ActorId, SenderId.NewId(Realm.Id));
    Sender twilio = new(new Phone("+15148422112", countryCode: "CA"), SenderHelper.GenerateTwilioSettings(), isDefault: false, ActorId, SenderId.NewId(Realm.Id));
    await _senderRepository.SaveAsync([sendGrid, twilio]);

    Assert.NotNull(sendGrid.Email);
    SearchSendersPayload payload = new()
    {
      Ids = [_sendGrid.EntityId, _twilio.EntityId, twilio.EntityId, Guid.Empty],
      Search = new TextSearch([new SearchTerm("%514%"), new SearchTerm(sendGrid.Email.Address)], SearchOperator.Or),
      Sort = [new SenderSortOption(SenderSort.PhoneNumber, isDescending: true)],
      Skip = 1,
      Limit = 1
    };
    SearchResults<SenderDto> results = await _senderService.SearchAsync(payload);
    Assert.Equal(2, results.Total);

    SenderDto sender = Assert.Single(results.Items);
    Assert.Equal(twilio.EntityId, sender.Id);
  }

  [Fact(DisplayName = "It should return the correct search results (Kind).")]
  public async Task Given_Kind_When_Search_Then_CorrectResults()
  {
    SearchSendersPayload payload = new()
    {
      Kind = SenderKind.Email
    };
    SearchResults<SenderDto> results = await _senderService.SearchAsync(payload);
    Assert.Equal(1, results.Total);

    SenderDto sender = Assert.Single(results.Items);
    Assert.Equal(_sendGrid.EntityId, sender.Id);
  }

  [Fact(DisplayName = "It should return the correct search results (Provider).")]
  public async Task Given_Provider_When_Search_Then_CorrectResults()
  {
    SearchSendersPayload payload = new()
    {
      Provider = SenderProvider.Twilio
    };
    SearchResults<SenderDto> results = await _senderService.SearchAsync(payload);
    Assert.Equal(1, results.Total);

    SenderDto sender = Assert.Single(results.Items);
    Assert.Equal(_twilio.EntityId, sender.Id);
  }

  [Fact(DisplayName = "It should throw TooManyResultsException when multiple senders were read.")]
  public async Task Given_MultipleResults_When_Read_Then_TooManyResultsException()
  {
    var exception = await Assert.ThrowsAsync<TooManyResultsException<SenderDto>>(async () => await _senderService.ReadAsync(_sendGrid.EntityId, SenderKind.Phone));
    Assert.Equal(1, exception.ExpectedCount);
    Assert.Equal(2, exception.ActualCount);
  }

  [Fact(DisplayName = "It should update an existing sender.")]
  public async Task Given_Exists_When_Update_Then_Updated()
  {
    UpdateSenderPayload payload = new()
    {
      Email = new EmailPayload($" {Faker.Internet.Email()} "),
      DisplayName = new Change<string>($" {Faker.Company.CompanyName()} "),
      Description = new Change<string>("  This is the default SendGrid sender.  ")
    };
    SenderDto? sender = await _senderService.UpdateAsync(_sendGrid.EntityId, payload);
    Assert.NotNull(sender);

    Assert.Equal(_sendGrid.EntityId, sender.Id);
    Assert.Equal(_sendGrid.Version + 1, sender.Version);
    Assert.Equal(Actor, sender.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, sender.UpdatedOn.AsUniversalTime(), TimeSpan.FromSeconds(10));

    Assert.Equal(payload.Email.Address.Trim(), sender.Email?.Address);
    Assert.Equal(payload.DisplayName.Value?.Trim(), sender.DisplayName);
    Assert.Equal(payload.Description.Value?.Trim(), sender.Description);
    Assert.Equal(new SendGridSettingsDto((SendGridSettings)_sendGrid.Settings), sender.SendGrid);
  }
}
