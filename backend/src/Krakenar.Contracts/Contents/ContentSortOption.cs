using Krakenar.Contracts.Search;

namespace Krakenar.Contracts.Contents;

public record ContentSortOption : SortOption
{
  public new ContentSort Field
  {
    get => Enum.Parse<ContentSort>(base.Field);
    set => base.Field = value.ToString();
  }

  public ContentSortOption() : this(ContentSort.UpdatedOn, isDescending: true)
  {
  }

  public ContentSortOption(ContentSort field, bool isDescending = false) : base(field.ToString(), isDescending)
  {
  }
}
