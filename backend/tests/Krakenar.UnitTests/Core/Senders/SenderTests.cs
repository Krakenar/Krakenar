using Bogus;
using Krakenar.Contracts.Senders;
using Krakenar.Core.Realms;
using Krakenar.Core.Senders.Events;
using Krakenar.Core.Senders.Settings;
using Krakenar.Core.Users;
using Logitar.EventSourcing;

namespace Krakenar.Core.Senders;

[Trait(Traits.Category, Categories.Unit)]
public class SenderTests
{
  private readonly Faker _faker = new();
  private readonly Sender _sendGrid;
  private readonly Sender _twilio;

  public SenderTests()
  {
    _sendGrid = new(new Email(_faker.Person.Email), SenderHelper.GenerateSendGridSettings(), isDefault: true);
    _twilio = new(new Phone("+15148454636", countryCode: "CA"), SenderHelper.GenerateTwilioSettings(), isDefault: true);
  }

  [Fact(DisplayName = "Delete: it should delete the sender.")]
  public void Given_Sender_When_Delete_Then_Deleted()
  {
    Assert.False(_sendGrid.IsDeleted);

    _sendGrid.Delete();
    Assert.True(_sendGrid.IsDeleted);
    Assert.Contains(_sendGrid.Changes, change => change is SenderDeleted);

    _sendGrid.ClearChanges();
    _sendGrid.Delete();
    Assert.False(_sendGrid.HasChanges);
    Assert.Empty(_sendGrid.Changes);
  }

  [Fact(DisplayName = "It should construct a new email sender from arguments.")]
  public void Given_Email_When_ctor_Then_Sender()
  {
    ActorId actorId = new(Guid.Parse("aa82fd25-286f-4049-bae8-001257a2d5ca"));
    RealmId realmId = new(Guid.Parse("c14d2cc6-1514-47f8-ada9-4e2974c09295"));
    SenderId senderId = new(Guid.Parse("f26093a8-a963-49f6-adc1-f183c44625c2"), realmId);

    Assert.NotNull(_sendGrid.Email);
    Sender sender = new(_sendGrid.Email, _sendGrid.Settings, isDefault: true, actorId, senderId);

    Assert.Equal(SenderKind.Email, sender.Kind);
    Assert.True(sender.IsDefault);
    Assert.Equal(_sendGrid.Email, sender.Email);
    Assert.Null(sender.Phone);
    Assert.Equal(_sendGrid.Provider, sender.Provider);
    Assert.Equal(_sendGrid.Settings, sender.Settings);
    Assert.Equal(actorId, sender.CreatedBy);
    Assert.Equal(actorId, sender.UpdatedBy);

    Assert.Equal(realmId, sender.RealmId);
    Assert.Equal(senderId, sender.Id);
  }

  [Fact(DisplayName = "It should construct a new phone sender from arguments.")]
  public void Given_Phone_When_ctor_Then_Sender()
  {
    Phone phone = new("1 (514) 845-4636", countryCode: "CA");
    TwilioSettings settings = SenderHelper.GenerateTwilioSettings();
    Sender sender = new(phone, settings);

    Assert.Equal(SenderKind.Phone, sender.Kind);
    Assert.False(sender.IsDefault);
    Assert.Null(sender.Email);
    Assert.Equal(phone, sender.Phone);
    Assert.Equal(settings.Provider, sender.Provider);
    Assert.Equal(settings, sender.Settings);
    Assert.Null(sender.CreatedBy);
    Assert.Null(sender.UpdatedBy);

    Assert.Null(sender.RealmId);
    Assert.NotEqual(Guid.Empty, sender.EntityId);
  }

  [Fact(DisplayName = "It should handle Description change correctly.")]
  public void Given_Changes_When_Description_Then_Changed()
  {
    Description description = new("  This is the administration sender.  ");
    _sendGrid.Description = description;
    _sendGrid.Update(_sendGrid.CreatedBy);
    Assert.Equal(description, _sendGrid.Description);
    Assert.Contains(_sendGrid.Changes, change => change is SenderUpdated updated && updated.Description?.Value is not null && updated.Description.Value.Equals(description));

    _sendGrid.ClearChanges();

    _sendGrid.Description = description;
    _sendGrid.Update();
    Assert.False(_sendGrid.HasChanges);
  }

  [Fact(DisplayName = "It should handle DisplayName change correctly.")]
  public void Given_Changes_When_DisplayName_Then_Changed()
  {
    DisplayName displayName = new(" Administrator ");
    _sendGrid.DisplayName = displayName;
    _sendGrid.Update(_sendGrid.CreatedBy);
    Assert.Equal(displayName, _sendGrid.DisplayName);
    Assert.Contains(_sendGrid.Changes, change => change is SenderUpdated updated && updated.DisplayName?.Value is not null && updated.DisplayName.Value.Equals(displayName));

    _sendGrid.ClearChanges();

    _sendGrid.DisplayName = displayName;
    _sendGrid.Update();
    Assert.False(_sendGrid.HasChanges);
  }

  [Fact(DisplayName = "It should handle Email change correctly.")]
  public void Given_Changes_When_Email_Then_Changed()
  {
    Email email = new($" {_faker.Internet.Email()} ");
    _sendGrid.Email = email;
    _sendGrid.Update(_sendGrid.CreatedBy);
    Assert.Equal(email, _sendGrid.Email);
    Assert.Contains(_sendGrid.Changes, change => change is SenderUpdated updated && updated.Email is not null && updated.Email.Equals(email));

    _sendGrid.ClearChanges();

    _sendGrid.Email = email;
    _sendGrid.Update();
    Assert.False(_sendGrid.HasChanges);
  }

  [Fact(DisplayName = "It should handle Phone change correctly.")]
  public void Given_Changes_When_Phone_Then_Changed()
  {
    Phone phone = new("1 (514) 842-2112", countryCode: "CA");
    _twilio.Phone = phone;
    _twilio.Update(_twilio.CreatedBy);
    Assert.Equal(phone, _twilio.Phone);
    Assert.Contains(_twilio.Changes, change => change is SenderUpdated updated && updated.Phone is not null && updated.Phone.Equals(phone));

    _twilio.ClearChanges();

    _twilio.Phone = phone;
    _twilio.Update();
    Assert.False(_twilio.HasChanges);
  }

  [Fact(DisplayName = "It should throw ArgumentNullException when the sender contact and settings are not a valid combination.")]
  public void Given_InvalidCombination_When_ctor_Then_ArgumentNullException()
  {
    var exception = Assert.Throws<ArgumentNullException>(() => new Sender(new Email(_faker.Person.Email), SenderHelper.GenerateTwilioSettings()));
    Assert.Equal("phone", exception.ParamName);

    exception = Assert.Throws<ArgumentNullException>(() => new Sender(new Phone("1 (514) 845-4636", countryCode: "CA"), SenderHelper.GenerateSendGridSettings()));
    Assert.Equal("email", exception.ParamName);
  }

  [Fact(DisplayName = "It should throw InvalidOperationException when setting the email of a phone sender.")]
  public void Given_PhoneSender_When_setEmail_Then_InvalidOperationException()
  {
    Phone phone = new("+15148454636", countryCode: "CA");
    var exception = Assert.Throws<InvalidOperationException>(() => _sendGrid.Phone = phone);
    Assert.Equal("The phone of a 'Email' sender cannot be changed.", exception.Message);
  }

  [Fact(DisplayName = "It should throw InvalidOperationException when setting the phone of an email sender.")]
  public void Given_EmailSender_When_setPhone_Then_InvalidOperationException()
  {
    Email email = new(_faker.Person.Email);
    var exception = Assert.Throws<InvalidOperationException>(() => _twilio.Email = email);
    Assert.Equal("The email of a 'Phone' sender cannot be changed.", exception.Message);
  }

  [Fact(DisplayName = "It should throw SenderProviderNotSupported when the provider is not supported.")]
  public void Given_InvalidProvider_When_ctor_Then_SenderProviderNotSupported()
  {
    InvalidSettings settings = new();
    var exception = Assert.Throws<SenderProviderNotSupportedException>(() => new Sender(new Email(_faker.Person.Email), settings));
    Assert.Equal(settings.Provider, exception.SenderProvider);
  }

  [Fact(DisplayName = "SetDefault: it should handle changes correctly.")]
  public void Given_Changes_When_SetDefault_Then_Changed()
  {
    _sendGrid.ClearChanges();
    _sendGrid.SetDefault(_sendGrid.IsDefault);
    Assert.False(_sendGrid.HasChanges);
    Assert.Empty(_sendGrid.Changes);

    bool isDefault = !_sendGrid.IsDefault;
    _sendGrid.SetDefault(isDefault);
    Assert.Equal(isDefault, _sendGrid.IsDefault);
    Assert.Contains(_sendGrid.Changes, change => change is SenderSetDefault changed && changed.IsDefault == isDefault);
  }

  [Fact(DisplayName = "SetSettings: it should handle changes correctly (SendGrid).")]
  public void Given_Changes_When_SetSendGridSettings_Then_Changed()
  {
    _sendGrid.ClearChanges();
    _sendGrid.SetSettings((SendGridSettings)_sendGrid.Settings);
    Assert.False(_sendGrid.HasChanges);
    Assert.Empty(_sendGrid.Changes);

    SendGridSettings settings = SenderHelper.GenerateSendGridSettings();
    _sendGrid.SetSettings(settings);
    Assert.True(_sendGrid.HasChanges);
    Assert.Contains(_sendGrid.Changes, change => change is SendGridSettingsChanged changed && changed.Settings.Equals(settings));
  }

  [Fact(DisplayName = "SetSettings: it should handle changes correctly (Twilio).")]
  public void Given_Changes_When_SetTwilioSettings_Then_Changed()
  {
    _twilio.ClearChanges();
    _twilio.SetSettings((TwilioSettings)_twilio.Settings);
    Assert.False(_twilio.HasChanges);
    Assert.Empty(_twilio.Changes);

    TwilioSettings settings = SenderHelper.GenerateTwilioSettings();
    _twilio.SetSettings(settings);
    Assert.True(_twilio.HasChanges);
    Assert.Contains(_twilio.Changes, change => change is TwilioSettingsChanged changed && changed.Settings.Equals(settings));
  }

  [Fact(DisplayName = "SetSettings: it should throw SenderProviderMismatchException when the provider is not valid (SendGrid).")]
  public void Given_ProviderMismatch_When_SetSendGridSettings_Then_SenderProviderMismatchException()
  {
    TwilioSettings settings = SenderHelper.GenerateTwilioSettings();
    var exception = Assert.Throws<SenderProviderMismatchException>(() => _sendGrid.SetSettings(settings));
    Assert.Equal(_sendGrid.RealmId?.ToGuid(), exception.RealmId);
    Assert.Equal(_sendGrid.EntityId, exception.SenderId);
    Assert.Equal(SenderProvider.SendGrid, exception.ExpectedProvider);
    Assert.Equal(settings.Provider, exception.ActualProvider);
  }

  [Fact(DisplayName = "SetSettings: it should throw SenderProviderMismatchException when the provider is not valid (Twilio).")]
  public void Given_ProviderMismatch_When_SetTwilioettings_Then_SenderProviderMismatchException()
  {
    SendGridSettings settings = SenderHelper.GenerateSendGridSettings();
    var exception = Assert.Throws<SenderProviderMismatchException>(() => _twilio.SetSettings(settings));
    Assert.Equal(_twilio.RealmId?.ToGuid(), exception.RealmId);
    Assert.Equal(_twilio.EntityId, exception.SenderId);
    Assert.Equal(SenderProvider.Twilio, exception.ExpectedProvider);
    Assert.Equal(settings.Provider, exception.ActualProvider);
  }

  [Theory(DisplayName = "ToString: it should return the correct string representation (Email).")]
  [InlineData(false)]
  [InlineData(true)]
  public void Given_EmailSender_When_ToString_Then_CorrectString(bool withDisplayName)
  {
    DisplayName? displayName = withDisplayName ? new(_faker.Person.FullName) : null;
    _sendGrid.DisplayName = displayName;
    _sendGrid.Update(_sendGrid.CreatedBy);

    Assert.NotNull(_sendGrid.Email);
    if (withDisplayName)
    {
      Assert.StartsWith($"{displayName} <{_sendGrid.Email}>", _sendGrid.ToString());
    }
    else
    {
      Assert.StartsWith(_sendGrid.Email.Address, _sendGrid.ToString());
    }
  }

  [Theory(DisplayName = "ToString: it should return the correct string representation (Phone).")]
  [InlineData(false)]
  [InlineData(true)]
  public void Given_PhoneSender_When_ToString_Then_CorrectString(bool withDisplayName)
  {
    DisplayName? displayName = withDisplayName ? new(_faker.Person.Company.Name) : null;
    _twilio.DisplayName = displayName;
    _twilio.Update(_twilio.CreatedBy);

    Assert.NotNull(_twilio.Phone);
    if (withDisplayName)
    {
      Assert.StartsWith($"{displayName} <{_twilio.Phone}>", _twilio.ToString());
    }
    else
    {
      Assert.StartsWith(_twilio.Phone.FormatToE164(), _twilio.ToString());
    }
  }
}
