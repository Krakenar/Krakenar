using Krakenar.Contracts.Localization;
using Krakenar.Contracts.Search;
using Krakenar.Web.Models.Search;

namespace Krakenar.Web.Models.Language;

public record SearchLanguagesParameters : SearchParameters
{
  public virtual SearchLanguagesPayload ToPayload()
  {
    SearchLanguagesPayload payload = new();
    Fill(payload);

    foreach (SortOption sort in ((SearchPayload)payload).Sort)
    {
      if (Enum.TryParse(sort.Field, out LanguageSort field))
      {
        payload.Sort.Add(new LanguageSortOption(field, sort.IsDescending));
      }
    }

    return payload;
  }
}
