namespace Krakenar.Contracts.Sessions;

public record RenewSessionPayload
{
  public string RefreshToken { get; set; }

  public List<CustomAttribute> CustomAttributes { get; set; } = [];

  public RenewSessionPayload() : this(string.Empty)
  {
  }

  public RenewSessionPayload(string refreshToken, IEnumerable<CustomAttribute>? customAttributes = null)
  {
    RefreshToken = refreshToken;

    if (customAttributes is not null)
    {
      CustomAttributes.AddRange(customAttributes);
    }
  }
}
