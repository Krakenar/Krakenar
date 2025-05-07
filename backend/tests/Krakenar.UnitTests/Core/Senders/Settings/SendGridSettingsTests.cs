using Krakenar.Contracts.Senders;
using SendGridSettingsDto = Krakenar.Contracts.Senders.Settings.SendGridSettings;

namespace Krakenar.Core.Senders.Settings;

[Trait(Traits.Category, Categories.Unit)]
public class SendGridSettingsTests
{
  [Fact(DisplayName = "It should construct the correct instance from another instance.")]
  public void Given_Instance_When_ctor_Then_Instance()
  {
    SendGridSettingsDto instance = new(SenderHelper.GenerateSendGridApiKey());

    SendGridSettings settings = new(instance);

    Assert.Equal(SenderProvider.SendGrid, settings.Provider);
    Assert.Equal(instance.ApiKey, settings.ApiKey);
  }

  [Fact(DisplayName = "It should construct the correct instance from arguments.")]
  public void Given_Arguments_When_ctor_Then_Instance()
  {
    string apiKey = SenderHelper.GenerateSendGridApiKey();

    SendGridSettings settings = new(apiKey);

    Assert.Equal(SenderProvider.SendGrid, settings.Provider);
    Assert.Equal(apiKey, settings.ApiKey);
  }

  [Fact(DisplayName = "It should throw ValidationException when the arguments are not valid.")]
  public void Given_InvalidArguments_When_ctor_Then_ValidationException()
  {
    var exception = Assert.Throws<FluentValidation.ValidationException>(() => new SendGridSettings(string.Empty));
    Assert.Single(exception.Errors);
    Assert.Contains(exception.Errors, e => e.ErrorCode == "NotEmptyValidator" && e.PropertyName == "ApiKey");
  }
}
