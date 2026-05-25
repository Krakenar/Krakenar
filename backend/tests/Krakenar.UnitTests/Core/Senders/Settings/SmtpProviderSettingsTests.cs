using Krakenar.Contracts.Senders;
using SmtpProviderSettingsDto = Krakenar.Contracts.Senders.Settings.SmtpProviderSettings;
using SmtpSecurityMode = Krakenar.Contracts.Senders.Settings.SmtpSecurityMode;

namespace Krakenar.Core.Senders.Settings;

[Trait(Traits.Category, Categories.Unit)]
public class SmtpProviderSettingsTests
{
  [Fact(DisplayName = "It should construct the correct instance from another instance.")]
  public void Given_Instance_When_ctor_Then_Instance()
  {
    SmtpProviderSettingsDto instance = new(SenderHelper.GenerateSmtpProviderSettings());

    SmtpProviderSettings settings = new(instance);

    Assert.Equal(SenderProvider.SmtpProvider, settings.Provider);
    Assert.Equal(instance.Host, settings.Host);
    Assert.Equal(instance.Port, settings.Port);
    Assert.Equal(instance.Username, settings.Username);
    Assert.Equal(instance.Password, settings.Password);
  }

  [Fact(DisplayName = "It should construct the correct instance from arguments.")]
  public void Given_Arguments_When_ctor_Then_Instance()
  {
    string host = "smtp.example.com";
    ushort port = 587;
    string username = "myuser";
    string password = "mypassword";

    SmtpProviderSettings settings = new(host, port, SmtpSecurityMode.Auto, username, password);

    Assert.Equal(SenderProvider.SmtpProvider, settings.Provider);
    Assert.Equal(host, settings.Host);
    Assert.Equal(port, settings.Port);
    Assert.Equal(username, settings.Username);
    Assert.Equal(password, settings.Password);
  }

  [Fact(DisplayName = "It should throw ValidationException when the arguments are not valid.")]
  public void Given_InvalidArguments_When_ctor_Then_ValidationException()
  {
    var exception = Assert.Throws<FluentValidation.ValidationException>(() => new SmtpProviderSettings(string.Empty, 0, (SmtpSecurityMode)(-1), string.Empty, string.Empty));
    Assert.Equal(4, exception.Errors.Count());
    Assert.Contains(exception.Errors, e => e.ErrorCode == "NotEmptyValidator" && e.PropertyName == "Host");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "EnumValidator" && e.PropertyName == "Security");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "NotEmptyValidator" && e.PropertyName == "Username");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "NotEmptyValidator" && e.PropertyName == "Password");
  }
}
