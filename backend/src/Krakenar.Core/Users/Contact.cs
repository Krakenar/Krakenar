using Krakenar.Contracts.Users;

namespace Krakenar.Core.Users;

public abstract record Contact : IContact
{
  public bool IsVerified { get; }

  protected Contact(bool isVerified)
  {
    IsVerified = isVerified;
  }
}
