namespace Krakenar.Core;

[Trait(Traits.Category, Categories.Unit)]
public class ExceptionExtensionsTests
{
  [Fact(DisplayName = "Unwrap: it should return the argument exception when it is a TargetInvocationException with null inner exception.")]
  public void Given_TargetInvocationNullInner_When_Unwrap_Then_ArgumentReturned()
  {
    TargetInvocationException argument = new(inner: null);
    Exception result = argument.Unwrap();
    Assert.Same(argument, result);
  }

  [Fact(DisplayName = "Unwrap: it should return the argument exception when it is not a TargetInvocationException.")]
  public void Given_NotTargetInvocation_When_Unwrap_Then_ArgumentReturned()
  {
    ArgumentException argument = new();
    Exception result = argument.Unwrap();
    Assert.Same(argument, result);
  }

  [Fact(DisplayName = "Unwrap: it should return the inner exception when the argument is a TargetInvocationException with non-null inner exception.")]
  public void Given_TargetInvocationWithInner_When_Unwrap_Then_InnerExceptionReturned()
  {
    ArgumentException inner = new();
    TargetInvocationException argument = new(inner);
    Exception result = argument.Unwrap();
    Assert.Same(inner, result);
  }

  [Fact(DisplayName = "Unwrap: it should return the innermost exception when the argument is a nested TargetInvocationException.")]
  public void Given_NestedTargetInvocations_When_Unwrap_Then_InnermostExceptionReturned()
  {
    InvalidOperationException innermost = new();
    TargetInvocationException middle = new(innermost);
    TargetInvocationException outer = new(middle);
    Exception result = outer.Unwrap();
    Assert.Same(innermost, result);
  }
}
