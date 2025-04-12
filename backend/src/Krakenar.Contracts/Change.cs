namespace Krakenar.Contracts;

public record Change<T>
{
  public T? Value { get; set; }

  public Change()
  {
  }

  public Change(T? value)
  {
    Value = value;
  }
}
