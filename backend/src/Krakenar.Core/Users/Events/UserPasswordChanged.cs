using Krakenar.Core.Passwords;

namespace Krakenar.Core.Users.Events;

public record UserPasswordChanged : UserPasswordEvent
{
  public UserPasswordChanged(Password password) : base(password)
  {
  }
}
