using Krakenar.Contracts.Senders;
using Krakenar.Core.Senders.Settings;

namespace Krakenar.Infrastructure.Messages.Providers;

public interface IProviderStrategy
{
  SenderProvider Provider { get; }

  IMessageHandler Execute(SenderSettings settings);
}
