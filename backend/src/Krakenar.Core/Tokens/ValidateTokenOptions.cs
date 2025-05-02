namespace Krakenar.Core.Tokens;

public record ValidateTokenOptions
{
  public List<string> ValidTypes { get; set; } = [];

  public List<string> ValidAudiences { get; set; } = [];
  public List<string> ValidIssuers { get; set; } = [];

  public bool Consume { get; set; }
}
