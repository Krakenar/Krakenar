using Krakenar.Contracts.Search;
using Krakenar.Contracts.Templates;
using Krakenar.Web.Models.Search;
using Microsoft.AspNetCore.Mvc;

namespace Krakenar.Web.Models.Template;

public record SearchTemplatesParameters : SearchParameters
{
  [FromQuery(Name = "type")]
  public string? ContentType { get; set; }

  public virtual SearchTemplatesPayload ToPayload()
  {
    SearchTemplatesPayload payload = new()
    {
      ContentType = ContentType
    };
    Fill(payload);

    foreach (SortOption sort in ((SearchPayload)payload).Sort)
    {
      if (Enum.TryParse(sort.Field, out TemplateSort field))
      {
        payload.Sort.Add(new TemplateSortOption(field, sort.IsDescending));
      }
    }

    return payload;
  }
}
