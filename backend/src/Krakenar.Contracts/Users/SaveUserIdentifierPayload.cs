namespace Krakenar.Contracts.Users;

public record SaveUserIdentifierPayload
{
  public string Value { get; set; }

  public SaveUserIdentifierPayload() : this(string.Empty)
  {
  }

  public SaveUserIdentifierPayload(string value)
  {
    Value = value;
  }
}
