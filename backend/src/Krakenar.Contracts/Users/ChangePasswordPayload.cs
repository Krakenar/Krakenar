namespace Krakenar.Contracts.Users;

public record ChangePasswordPayload
{
  public string? Current { get; set; }
  public string New { get; set; }

  public ChangePasswordPayload() : this(string.Empty)
  {
  }

  public ChangePasswordPayload(string @new, string? current = null)
  {
    Current = current;
    New = @new;
  }
}
