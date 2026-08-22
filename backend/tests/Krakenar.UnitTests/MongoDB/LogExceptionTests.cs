using Logitar;

namespace Krakenar.MongoDB;

[Trait(Traits.Category, Categories.Unit)]
public class LogExceptionTests
{
  [Fact(DisplayName = "It should map an exception without an inner exception.")]
  public void Given_NoInnerException_When_ctor_Then_Mapped()
  {
    Exception exception = new InvalidOperationException("Something went wrong.");
    exception.Data["Key"] = "Value";

    LogException logException = new(exception);

    Assert.NotEqual(Guid.Empty, logException.UniqueId);
    Assert.Equal(exception.GetType().GetNamespaceQualifiedName(), logException.Type);
    Assert.Equal(exception.Message, logException.Message);
    Assert.Equal(exception.HResult, logException.HResult);
    Assert.Equal("Value", Assert.Contains("Key", logException.Data));
    Assert.Null(logException.InnerException);
  }

  [Fact(DisplayName = "It should map nested inner exceptions recursively.")]
  public void Given_InnerExceptions_When_ctor_Then_MappedRecursively()
  {
    Exception rootCause = new ArgumentException("Invalid argument.");
    Exception middle = new InvalidOperationException("Operation failed.", rootCause);
    Exception outer = new ApplicationException("Request failed.", middle);

    LogException logException = new(outer);

    Assert.Equal(outer.GetType().GetNamespaceQualifiedName(), logException.Type);
    Assert.Equal(outer.Message, logException.Message);

    Assert.NotNull(logException.InnerException);
    Assert.Equal(middle.GetType().GetNamespaceQualifiedName(), logException.InnerException.Type);
    Assert.Equal(middle.Message, logException.InnerException.Message);

    Assert.NotNull(logException.InnerException.InnerException);
    Assert.Equal(rootCause.GetType().GetNamespaceQualifiedName(), logException.InnerException.InnerException.Type);
    Assert.Equal(rootCause.Message, logException.InnerException.InnerException.Message);
    Assert.Null(logException.InnerException.InnerException.InnerException);
  }
}
