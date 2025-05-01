using Krakenar.Contracts.Dictionaries;
using Krakenar.Contracts.Search;
using Krakenar.Web.Models.Search;

namespace Krakenar.Web.Models.Dictionary;

public record SearchDictionariesParameters : SearchParameters
{
  public virtual SearchDictionariesPayload ToPayload()
  {
    SearchDictionariesPayload payload = new();
    Fill(payload);

    foreach (SortOption sort in ((SearchPayload)payload).Sort)
    {
      if (Enum.TryParse(sort.Field, out DictionarySort field))
      {
        payload.Sort.Add(new DictionarySortOption(field, sort.IsDescending));
      }
    }

    return payload;
  }
}
