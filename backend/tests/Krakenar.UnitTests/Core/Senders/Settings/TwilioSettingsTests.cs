using Krakenar.Contracts.Senders;
using TwilioSettingsDto = Krakenar.Contracts.Senders.Settings.TwilioSettings;

namespace Krakenar.Core.Senders.Settings;

[Trait(Traits.Category, Categories.Unit)]
public class TwilioSettingsTests
{
  [Fact(DisplayName = "It should construct the correct instance from another instance.")]
  public void Given_Instance_When_ctor_Then_Instance()
  {
    TwilioSettingsDto instance = new(SenderHelper.GenerateTwilioAccountSid(), SenderHelper.GenerateTwilioAuthenticationToken());

    TwilioSettings settings = new(instance);

    Assert.Equal(SenderProvider.Twilio, settings.Provider);
    Assert.Equal(instance.AccountSid, settings.AccountSid);
    Assert.Equal(instance.AuthenticationToken, settings.AuthenticationToken);
  }

  [Fact(DisplayName = "It should construct the correct instance from arguments.")]
  public void Given_Arguments_When_ctor_Then_Instance()
  {
    string accountSid = SenderHelper.GenerateTwilioAccountSid();
    string authenticationToken = SenderHelper.GenerateTwilioAuthenticationToken();

    TwilioSettings settings = new(accountSid, authenticationToken);

    Assert.Equal(SenderProvider.Twilio, settings.Provider);
    Assert.Equal(accountSid, settings.AccountSid);
    Assert.Equal(authenticationToken, settings.AuthenticationToken);
  }

  [Fact(DisplayName = "It should throw ValidationException when the arguments are not valid.")]
  public void Given_InvalidArguments_When_ctor_Then_ValidationException()
  {
    var exception = Assert.Throws<FluentValidation.ValidationException>(() => new TwilioSettings(string.Empty, string.Empty));
    Assert.Equal(2, exception.Errors.Count());
    Assert.Contains(exception.Errors, e => e.ErrorCode == "NotEmptyValidator" && e.PropertyName == "AccountSid");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "NotEmptyValidator" && e.PropertyName == "AuthenticationToken");
  }
}
