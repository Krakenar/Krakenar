using Krakenar.Contracts.Senders;
using Krakenar.Core.Senders.Settings;

namespace Krakenar.Core.Senders;

public static class SenderExtensions
{
  public static SenderKind GetSenderKind(this SenderSettings settings) => settings.Provider.GetSenderKind();
  public static SenderKind GetSenderKind(this SenderProvider provider) => provider switch
  {
    SenderProvider.SendGrid => SenderKind.Email,
    SenderProvider.Twilio => SenderKind.Phone,
    _ => throw new SenderProviderNotSupportedException(provider),
  };
}
