using Microsoft.IdentityModel.Tokens;

namespace Krakenar.Core.Tokens;

public class SecurityTokenBlacklistedException : SecurityTokenValidationException
{
  private const string ErrorMessage = "The security token is blacklisted.";

  public IReadOnlyCollection<string> BlacklistedIds
  {
    get => (IReadOnlyCollection<string>)Data[nameof(BlacklistedIds)]!;
    private set => Data[nameof(BlacklistedIds)] = value;
  }

  public SecurityTokenBlacklistedException(IEnumerable<string> blacklistedIds) : base(BuildMessage(blacklistedIds))
  {
    BlacklistedIds = blacklistedIds.ToList().AsReadOnly();
  }

  private static string BuildMessage(IEnumerable<string> blacklistedIds)
  {
    StringBuilder message = new();
    message.AppendLine(ErrorMessage);
    message.Append(nameof(BlacklistedIds)).Append(':').AppendLine();
    foreach (string blacklistedId in blacklistedIds)
    {
      message.Append(" - ").Append(blacklistedId).AppendLine();
    }
    return message.ToString();
  }
}
