using Krakenar.Core.Passwords;

namespace Krakenar.Core.Users.Events;

public record UserPasswordReset : UserPasswordEvent
{
  public UserPasswordReset(Password password) : base(password)
  {
  }
}
