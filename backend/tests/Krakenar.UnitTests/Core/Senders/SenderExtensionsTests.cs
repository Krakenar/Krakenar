using Krakenar.Contracts.Senders;

namespace Krakenar.Core.Senders;

[Trait(Traits.Category, Categories.Unit)]
public class SenderExtensionsTests
{
  [Fact(DisplayName = "GetSenderKind: it should return the correct kind from each sender provider.")]
  public void Given_SenderProvider_When_GetSenderKind_Then_CorrectKind()
  {
    Dictionary<SenderProvider, SenderKind> kindByProviders = new()
    {
      [SenderProvider.SendGrid] = SenderKind.Email,
      [SenderProvider.Twilio] = SenderKind.Phone
    };

    SenderProvider[] providers = Enum.GetValues<SenderProvider>();
    foreach (SenderProvider provider in providers)
    {
      Assert.Equal(kindByProviders[provider], provider.GetSenderKind());
    }
  }

  [Fact(DisplayName = "GetSenderKind: it should return the correct kind from SendGridSettings.")]
  public void Given_SenderSettings_When_GetSenderKind_Then_CorrectKind()
  {
    Assert.Equal(SenderKind.Email, SenderHelper.GenerateSendGridSettings().GetSenderKind());
  }

  [Fact(DisplayName = "GetSenderKind: it should return the correct kind from TwilioSettings.")]
  public void Given_TwilioSettings_When_GetSenderKind_Then_CorrectKind()
  {
    Assert.Equal(SenderKind.Phone, SenderHelper.GenerateTwilioSettings().GetSenderKind());
  }
}
