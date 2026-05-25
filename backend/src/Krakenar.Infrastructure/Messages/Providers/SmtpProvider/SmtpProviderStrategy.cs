using Krakenar.Contracts.Senders;
using Krakenar.Core.Senders.Settings;

namespace Krakenar.Infrastructure.Messages.Providers.SmtpProvider;

internal class SmtpProviderStrategy : IProviderStrategy
{
  public SenderProvider Provider { get; } = SenderProvider.SmtpProvider;

  public IMessageHandler Execute(SenderSettings settings)
  {
    return new SmtpProviderHandler((SmtpProviderSettings)settings);
  }
}
