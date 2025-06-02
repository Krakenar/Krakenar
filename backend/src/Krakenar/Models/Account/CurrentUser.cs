using SessionDto = Krakenar.Contracts.Sessions.Session;
using UserDto = Krakenar.Contracts.Users.User;

namespace Krakenar.Models.Account;

public record CurrentUser
{
  public Guid Id { get; set; }
  public Guid SessionId { get; set; }

  public string DisplayName { get; set; }
  public string? EmailAddress { get; set; }
  public string? PhoneNumber { get; set; }
  public string? PictureUrl { get; set; }

  public CurrentUser() : this(string.Empty)
  {
  }

  public CurrentUser(string displayName)
  {
    DisplayName = displayName;
  }

  public CurrentUser(SessionDto session) : this(session.User)
  {
    SessionId = session.Id;
  }

  public CurrentUser(UserDto user)
  {
    Id = user.Id;

    DisplayName = user.FullName ?? user.UniqueName;
    EmailAddress = user.Email?.Address;
    PhoneNumber = user.Phone?.E164Formatted;
    PictureUrl = user.Picture;
  }
}
