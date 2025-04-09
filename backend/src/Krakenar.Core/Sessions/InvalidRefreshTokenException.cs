using Krakenar.Contracts;
using Logitar;

namespace Krakenar.Core.Sessions;

public class InvalidRefreshTokenException : InvalidCredentialsException
{
  private const string ErrorMessage = "The specified value is not a valid refresh token.";

  public string RefreshToken
  {
    get => (string)Data[nameof(RefreshToken)]!;
    private set => Data[nameof(RefreshToken)] = value;
  }
  public string PropertyName
  {
    get => (string)Data[nameof(PropertyName)]!;
    private set => Data[nameof(PropertyName)] = value;
  }

  public override Error Error
  {
    get
    {
      Error error = new(this.GetErrorCode(), ErrorMessage);
      error.Data[nameof(RefreshToken)] = RefreshToken;
      error.Data[nameof(PropertyName)] = PropertyName;
      return error;
    }
  }

  public InvalidRefreshTokenException(string refreshToken, string propertyName, Exception? innerException = null)
    : base(BuildMessage(refreshToken, propertyName), innerException)
  {
    RefreshToken = refreshToken;
    PropertyName = propertyName;
  }

  private static string BuildMessage(string refreshToken, string propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(RefreshToken), refreshToken)
    .AddData(nameof(PropertyName), propertyName)
    .Build();
}
