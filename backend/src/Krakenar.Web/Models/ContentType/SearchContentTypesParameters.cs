using Krakenar.Contracts.Contents;
using Krakenar.Contracts.Search;
using Krakenar.Web.Models.Search;
using Microsoft.AspNetCore.Mvc;

namespace Krakenar.Web.Models.ContentType;

public record SearchContentTypesParameters : SearchParameters
{
  [FromQuery(Name = "invariant")]
  public bool? IsInvariant { get; set; }

  public virtual SearchContentTypesPayload ToPayload()
  {
    SearchContentTypesPayload payload = new()
    {
      IsInvariant = IsInvariant
    };
    Fill(payload);

    foreach (SortOption sort in ((SearchPayload)payload).Sort)
    {
      if (Enum.TryParse(sort.Field, out ContentTypeSort content))
      {
        payload.Sort.Add(new ContentTypeSortOption(content, sort.IsDescending));
      }
    }

    return payload;
  }
}
