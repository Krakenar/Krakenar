using Krakenar.Contracts.Users;

namespace Krakenar.Contracts.Messages;

public record Recipient
{
  public Guid Id { get; set; }

  public RecipientType Type { get; set; }

  public Email? Email { get; set; }
  public Phone? Phone { get; set; }
  public string? DisplayName { get; set; }

  public User? User { get; set; }
}
