using Krakenar.Contracts.Senders;
using Krakenar.Core.Senders.Settings;

namespace Krakenar.Infrastructure.Messages.Providers.SendGrid;

internal class SendGridStrategy : IProviderStrategy
{
  public SenderProvider Provider { get; } = SenderProvider.SendGrid;

  public IMessageHandler Execute(SenderSettings settings)
  {
    return new SendGridHandler((SendGridSettings)settings);
  }
}
