namespace Krakenar.Contracts.Passwords;

public class CreateOneTimePasswordPayload
{
  public Guid? Id { get; set; }
  public string? User { get; set; }

  public string Characters { get; set; }
  public int Length { get; set; }

  public DateTime? ExpiresOn { get; set; }
  public int? MaximumAttempts { get; set; }

  public List<CustomAttribute> CustomAttributes { get; set; } = [];

  public CreateOneTimePasswordPayload() : this(string.Empty, length: 0)
  {
  }

  public CreateOneTimePasswordPayload(string characters, int length)
  {
    Characters = characters;
    Length = length;
  }
}
