namespace Krakenar.Contracts.Users;

public abstract record ContactPayload : IContact
{
  public bool IsVerified { get; set; }

  protected ContactPayload() : this(false)
  {
  }

  protected ContactPayload(bool isVerified)
  {
    IsVerified = isVerified;
  }
}
