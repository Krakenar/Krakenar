using Krakenar.Core.Senders.Settings;
using Logitar;

namespace Krakenar;

public static class SenderHelper
{
  public static string GenerateSendGridApiKey()
  {
    Guid id = Guid.NewGuid();
    byte[] secret = RandomNumberGenerator.GetBytes(256 / 8);
    return string.Join('.', "SG", Convert.ToBase64String(id.ToByteArray()).ToUriSafeBase64(), Convert.ToBase64String(secret).ToUriSafeBase64());
  }
  public static SendGridSettings GenerateSendGridSettings() => new(GenerateSendGridApiKey());

  public static string GenerateTwilioAccountSid() => string.Concat("AC", Guid.NewGuid().ToString("N"));
  public static string GenerateTwilioAuthenticationToken() => Guid.NewGuid().ToString("N");
  public static TwilioSettings GenerateTwilioSettings() => new(GenerateTwilioAccountSid(), GenerateTwilioAuthenticationToken());
}
