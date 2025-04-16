namespace Krakenar.Contracts.Users;

public record CreateOrReplaceUserResult
{
  public User? User { get; set; }
  public bool Created { get; set; }

  public CreateOrReplaceUserResult()
  {
  }

  public CreateOrReplaceUserResult(User? user, bool created)
  {
    User = user;
    Created = created;
  }
}
