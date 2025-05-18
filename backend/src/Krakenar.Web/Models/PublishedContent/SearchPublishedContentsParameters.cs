using Krakenar.Contracts.Contents;
using Krakenar.Contracts.Search;
using Microsoft.AspNetCore.Mvc;

namespace Krakenar.Web.Models.PublishedContent;

public record SearchPublishedContentsParameters
{
  protected const char SortSeparator = '.';
  protected const string IsDescending = "DESC";

  [FromQuery(Name = "content_id")]
  public List<int> ContentIds { get; set; } = [];

  [FromQuery(Name = "content_uid")]
  public List<Guid> ContentUids { get; set; } = [];

  [FromQuery(Name = "language_id")]
  public List<int> LanguageIds { get; set; } = [];

  [FromQuery(Name = "language_uid")]
  public List<Guid> LanguageUids { get; set; } = [];

  [FromQuery(Name = "language_code")]
  public List<string> LanguageCodes { get; set; } = [];

  [FromQuery(Name = "language_default")]
  public bool? LanguageIsDefault { get; set; }

  [FromQuery(Name = "type_id")]
  public List<int> ContentTypeIds { get; set; } = [];

  [FromQuery(Name = "type_uid")]
  public List<Guid> ContentTypeUids { get; set; } = [];

  [FromQuery(Name = "type_name")]
  public List<string> ContentTypeNames { get; set; } = [];

  [FromQuery(Name = "search")]
  public List<string> SearchTerms { get; set; } = [];

  [FromQuery(Name = "search_operator")]
  public SearchOperator SearchOperator { get; set; }

  [FromQuery(Name = "sort")]
  public List<string> Sort { get; set; } = [];

  [FromQuery(Name = "skip")]
  public int Skip { get; set; }

  [FromQuery(Name = "limit")]
  public int Limit { get; set; }

  public SearchPublishedContentsPayload ToPayload()
  {
    SearchPublishedContentsPayload payload = new();
    payload.Content.Ids.AddRange(ContentIds);
    payload.Content.Uids.AddRange(ContentUids);
    payload.ContentType.Ids.AddRange(ContentTypeIds);
    payload.ContentType.Uids.AddRange(ContentTypeUids);
    payload.ContentType.Names.AddRange(ContentTypeNames);
    payload.Language.Ids.AddRange(LanguageIds);
    payload.Language.Uids.AddRange(LanguageUids);
    payload.Language.Codes.AddRange(LanguageCodes);
    payload.Language.IsDefault = LanguageIsDefault;

    foreach (string term in SearchTerms)
    {
      payload.Search.Terms.Add(new SearchTerm(term));
    }
    payload.Search.Operator = SearchOperator;

    foreach (string sort in Sort)
    {
      int index = sort.IndexOf(SortSeparator);
      string fieldValue = index >= 0 ? sort[(index + 1)..] : sort;
      bool isDescending = index >= 0 && sort[..index].Equals(IsDescending, StringComparison.InvariantCultureIgnoreCase);
      if (Enum.TryParse(fieldValue, out PublishedContentSort field))
      {
        payload.Sort.Add(new PublishedContentSortOption(field, isDescending));
      }
    }

    payload.Skip = Skip;
    payload.Limit = Limit;

    return payload;
  }
}
