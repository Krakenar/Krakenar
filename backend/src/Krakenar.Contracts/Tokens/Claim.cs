namespace Krakenar.Contracts.Tokens;

public record Claim
{
  public string Name { get; set; } = string.Empty;
  public string Value { get; set; } = string.Empty;
  public string? Type { get; set; }

  public Claim()
  {
  }

  public Claim(string name, string value, string? type = null)
  {
    Name = name;
    Value = value;
    Type = type;
  }
}
