using Krakenar.Core.Passwords;

namespace Krakenar.Core.Users.Events;

public record UserPasswordUpdated : UserPasswordEvent
{
  public UserPasswordUpdated(Password password) : base(password)
  {
  }
}
