using Krakenar.Contracts.Senders;

namespace Krakenar.Core.Senders.Settings;

public abstract record SenderSettings
{
  public abstract SenderProvider Provider { get; }
}
