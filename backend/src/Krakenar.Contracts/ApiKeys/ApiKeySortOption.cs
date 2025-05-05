using Krakenar.Contracts.Search;

namespace Krakenar.Contracts.ApiKeys;

public record ApiKeySortOption : SortOption
{
  public new ApiKeySort Field
  {
    get => Enum.Parse<ApiKeySort>(base.Field);
    set => base.Field = value.ToString();
  }

  public ApiKeySortOption() : this(ApiKeySort.UpdatedOn, isDescending: true)
  {
  }

  public ApiKeySortOption(ApiKeySort field, bool isDescending = false) : base(field.ToString(), isDescending)
  {
  }
}
