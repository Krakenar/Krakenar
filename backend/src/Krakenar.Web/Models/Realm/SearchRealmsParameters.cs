using Krakenar.Contracts.Realms;
using Krakenar.Contracts.Search;
using Krakenar.Web.Models.Search;

namespace Krakenar.Web.Models.Realm;

public record SearchRealmsParameters : SearchParameters
{
  public virtual SearchRealmsPayload ToPayload()
  {
    SearchRealmsPayload payload = new();
    Fill(payload);

    foreach (SortOption sort in ((SearchPayload)payload).Sort)
    {
      if (Enum.TryParse(sort.Field, out RealmSort field))
      {
        payload.Sort.Add(new RealmSortOption(field, sort.IsDescending));
      }
    }

    return payload;
  }
}
