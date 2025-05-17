using Krakenar.Contracts.Contents;
using Krakenar.Contracts.Search;
using Krakenar.Web.Models.Search;
using Microsoft.AspNetCore.Mvc;

namespace Krakenar.Web.Models.Content;

public record SearchContentLocalesParameters : SearchParameters
{
  [FromQuery(Name = "type")]
  public Guid? ContentTypeId { get; set; }

  [FromQuery(Name = "language")]
  public Guid? LanguageId { get; set; }

  public virtual SearchContentLocalesPayload ToPayload()
  {
    SearchContentLocalesPayload payload = new()
    {
      ContentTypeId = ContentTypeId,
      LanguageId = LanguageId
    };
    Fill(payload);

    foreach (SortOption sort in ((SearchPayload)payload).Sort)
    {
      if (Enum.TryParse(sort.Field, out ContentSort content))
      {
        payload.Sort.Add(new ContentSortOption(content, sort.IsDescending));
      }
    }

    return payload;
  }
}
