using Krakenar.Contracts.Search;
using Krakenar.Contracts.Sessions;
using Microsoft.AspNetCore.Mvc;

namespace Krakenar.Web.Models.Session;

public record SearchSessionsParameters : SearchParameters
{
  [FromQuery(Name = "user")]
  public virtual Guid? UserId { get; set; }

  [FromQuery(Name = "active")]
  public virtual bool? IsActive { get; set; }

  [FromQuery(Name = "persistent")]
  public virtual bool? IsPersistent { get; set; }

  public virtual SearchSessionsPayload ToPayload()
  {
    SearchSessionsPayload payload = new()
    {
      UserId = UserId,
      IsActive = IsActive,
      IsPersistent = IsPersistent
    };
    Fill(payload);

    foreach (SortOption sort in ((SearchPayload)payload).Sort)
    {
      if (Enum.TryParse(sort.Field, out SessionSort field))
      {
        payload.Sort.Add(new SessionSortOption(field, sort.IsDescending));
      }
    }

    return payload;
  }
}
