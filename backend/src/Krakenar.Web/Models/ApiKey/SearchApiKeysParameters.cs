using Krakenar.Contracts.ApiKeys;
using Krakenar.Contracts.Search;
using Krakenar.Web.Models.Search;
using Microsoft.AspNetCore.Mvc;

namespace Krakenar.Web.Models.ApiKey;

public record SearchApiKeysParameters : SearchParameters
{
  [FromQuery(Name = "authenticated")]
  public bool? HasAuthenticated { get; set; }

  [FromQuery(Name = "role")]
  public Guid? RoleId { get; set; }

  [FromQuery(Name = "expired")]
  public bool? IsExpired { get; set; }

  [FromQuery(Name = "moment")]
  public DateTime? Moment { get; set; }

  public virtual SearchApiKeysPayload ToPayload()
  {
    SearchApiKeysPayload payload = new()
    {
      HasAuthenticated = HasAuthenticated,
      RoleId = RoleId
    };
    if (IsExpired.HasValue)
    {
      payload.Status = new ApiKeyStatus(IsExpired.Value, Moment);
    }
    Fill(payload);

    foreach (SortOption sort in ((SearchPayload)payload).Sort)
    {
      if (Enum.TryParse(sort.Field, out ApiKeySort field))
      {
        payload.Sort.Add(new ApiKeySortOption(field, sort.IsDescending));
      }
    }

    return payload;
  }
}
