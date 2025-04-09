using Krakenar.Contracts.Actors;

namespace Krakenar.Contracts.Users;

public abstract record Contact : IContact
{
  public bool IsVerified { get; set; }
  public Actor? VerifiedBy { get; set; }
  public DateTime? VerifiedOn { get; set; }

  public Contact()
  {
  }

  public Contact(bool isVerified)
  {
    IsVerified = isVerified;
  }
}
