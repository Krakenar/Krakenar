using Krakenar.Contracts.Messages;
using Krakenar.Contracts.Search;
using Krakenar.Web.Models.Search;
using Microsoft.AspNetCore.Mvc;

namespace Krakenar.Web.Models.Message;

public record SearchMessagesParameters : SearchParameters
{
  [FromQuery(Name = "template")]
  public Guid? TemplateId { get; set; }

  [FromQuery(Name = "demo")]
  public bool? IsDemo { get; set; }

  [FromQuery(Name = "status")]
  public MessageStatus? Status { get; set; }

  public virtual SearchMessagesPayload ToPayload()
  {
    SearchMessagesPayload payload = new()
    {
      TemplateId = TemplateId,
      IsDemo = IsDemo,
      Status = Status
    };
    Fill(payload);

    foreach (SortOption sort in ((SearchPayload)payload).Sort)
    {
      if (Enum.TryParse(sort.Field, out MessageSort field))
      {
        payload.Sort.Add(new MessageSortOption(field, sort.IsDescending));
      }
    }

    return payload;
  }
}
