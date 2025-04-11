using Logitar;

namespace Krakenar.Infrastructure.Passwords;

public class PasswordStrategyNotSupportedException : NotSupportedException
{
  private const string ErrorMessage = "The specified password strategy is not supported.";

  public string Strategy
  {
    get => (string)Data[nameof(Strategy)]!;
    private set => Data[nameof(Strategy)] = value;
  }

  public PasswordStrategyNotSupportedException(string strategy) : base(BuildMessage(strategy))
  {
    Strategy = strategy;
  }

  private static string BuildMessage(string strategy) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(Strategy), strategy)
    .Build();
}
