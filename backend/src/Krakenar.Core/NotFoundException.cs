using Krakenar.Contracts;

namespace Krakenar.Core;

public abstract class NotFoundException : ErrorException
{
  protected NotFoundException(string? message, Exception? innerException = null) : base(message, innerException)
  {
  }
}
