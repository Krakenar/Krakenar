namespace Krakenar.Core;

public static class ExceptionExtensions
{
  public static Exception Unwrap(this Exception exception)
  {
    if (exception is TargetInvocationException && exception.InnerException is not null)
    {
      return exception.InnerException.Unwrap();
    }
    return exception;
  }
}
