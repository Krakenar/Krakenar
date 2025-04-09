using Krakenar.Contracts;

namespace Krakenar.Core;

public abstract class InvalidCredentialsException : ErrorException
{
  public InvalidCredentialsException(string? message, Exception? innerException = null) : base(message, innerException)
  {
  }
}
