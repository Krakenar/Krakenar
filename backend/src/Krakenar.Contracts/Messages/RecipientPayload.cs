namespace Krakenar.Contracts.Messages;

public record RecipientPayload
{
  public RecipientType Type { get; set; }

  public string? Address { get; set; }
  public string? DisplayName { get; set; }

  public string? PhoneNumber { get; set; }

  public Guid? UserId { get; set; }
}
