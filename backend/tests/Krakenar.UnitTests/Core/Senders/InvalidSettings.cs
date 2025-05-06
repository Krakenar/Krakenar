using Krakenar.Contracts.Senders;
using Krakenar.Core.Senders.Settings;

namespace Krakenar.Core.Senders;

public record InvalidSettings : SenderSettings
{
  public override SenderProvider Provider => (SenderProvider)(-1);
}
