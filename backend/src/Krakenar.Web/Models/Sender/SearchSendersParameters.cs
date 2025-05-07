using Krakenar.Contracts.Search;
using Krakenar.Contracts.Senders;
using Krakenar.Web.Models.Search;

namespace Krakenar.Web.Models.Sender;

public record SearchSendersParameters : SearchParameters
{
  [JsonPropertyName("kind")]
  public SenderKind? Kind { get; set; }

  [JsonPropertyName("provider")]
  public SenderProvider? Provider { get; set; }

  public virtual SearchSendersPayload ToPayload()
  {
    SearchSendersPayload payload = new()
    {
      Kind = Kind,
      Provider = Provider
    };
    Fill(payload);

    foreach (SortOption sort in ((SearchPayload)payload).Sort)
    {
      if (Enum.TryParse(sort.Field, out SenderSort field))
      {
        payload.Sort.Add(new SenderSortOption(field, sort.IsDescending));
      }
    }

    return payload;
  }
}
