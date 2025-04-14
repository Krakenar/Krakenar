using Krakenar.Contracts.Search;
using Krakenar.Contracts.Users;
using Krakenar.Web.Models.Search;
using Microsoft.AspNetCore.Mvc;

namespace Krakenar.Web.Models.User;

public record SearchUsersParameters : SearchParameters
{
  [FromQuery(Name = "password")]
  public bool? HasPassword { get; set; }

  [FromQuery(Name = "disabled")]
  public bool? IsDisabled { get; set; }

  [FromQuery(Name = "confirmed")]
  public bool? IsConfirmed { get; set; }

  [FromQuery(Name = "authenticated")]
  public bool? HasAuthenticated { get; set; }

  [FromQuery(Name = "role")]
  public Guid? RoleId { get; set; }

  public virtual SearchUsersPayload ToPayload()
  {
    SearchUsersPayload payload = new()
    {
      HasPassword = HasPassword,
      IsDisabled = IsDisabled,
      IsConfirmed = IsConfirmed,
      HasAuthenticated = HasAuthenticated,
      RoleId = RoleId
    };
    Fill(payload);

    foreach (SortOption sort in ((SearchPayload)payload).Sort)
    {
      if (Enum.TryParse(sort.Field, out UserSort field))
      {
        payload.Sort.Add(new UserSortOption(field, sort.IsDescending));
      }
    }

    return payload;
  }
}
