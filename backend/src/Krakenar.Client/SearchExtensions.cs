using Krakenar.Contracts.Search;

namespace Krakenar.Client;

public static class SearchExtensions
{
  public static Dictionary<string, List<object?>> ToQueryParameters(this SearchPayload payload)
  {
    Dictionary<string, List<object?>> parameters = [];

    parameters["ids"] = payload.Ids.Select(id => (object?)id).ToList();
    parameters["search"] = payload.Search.Terms.Select(term => (object?)term).ToList();
    parameters["search_operator"] = [payload.Search.Operator];
    parameters["sort"] = payload.Sort.Select(sort => (object?)(sort.IsDescending ? $"DESC.{sort.Field}" : sort.Field)).ToList();
    parameters["skip"] = [payload.Skip];
    parameters["limit"] = [payload.Limit];

    return parameters;
  }

  public static string? ToQueryString(this IEnumerable<KeyValuePair<string, List<object?>>> parameters)
  {
    List<string> formatted = [];

    foreach (KeyValuePair<string, List<object?>> parameter in parameters)
    {
      if (!string.IsNullOrWhiteSpace(parameter.Key))
      {
        string key = parameter.Key.Trim();
        foreach (object? value in parameter.Value)
        {
          string? stringValue = value?.ToString();
          if (!string.IsNullOrWhiteSpace(stringValue))
          {
            formatted.Add(string.Join('=', key, stringValue.Trim()));
          }
        }
      }
    }

    return formatted.Count < 1 ? null : string.Join('&', formatted);
  }
}
