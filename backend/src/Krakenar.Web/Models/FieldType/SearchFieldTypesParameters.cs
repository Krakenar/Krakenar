using Krakenar.Contracts.Fields;
using Krakenar.Contracts.Search;
using Krakenar.Web.Models.Search;
using Microsoft.AspNetCore.Mvc;

namespace Krakenar.Web.Models.FieldType;

public record SearchFieldTypesParameters : SearchParameters
{
  [FromQuery(Name = "type")]
  public DataType? DataType { get; set; }

  public virtual SearchFieldTypesPayload ToPayload()
  {
    SearchFieldTypesPayload payload = new()
    {
      DataType = DataType
    };
    Fill(payload);

    foreach (SortOption sort in ((SearchPayload)payload).Sort)
    {
      if (Enum.TryParse(sort.Field, out FieldTypeSort field))
      {
        payload.Sort.Add(new FieldTypeSortOption(field, sort.IsDescending));
      }
    }

    return payload;
  }
}
