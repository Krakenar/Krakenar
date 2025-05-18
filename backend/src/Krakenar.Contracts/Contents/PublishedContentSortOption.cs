using Krakenar.Contracts.Search;

namespace Krakenar.Contracts.Contents;

public record PublishedContentSortOption : SortOption
{
  public new PublishedContentSort Field
  {
    get => Enum.Parse<PublishedContentSort>(base.Field);
    set => base.Field = value.ToString();
  }

  public PublishedContentSortOption() : this(PublishedContentSort.DisplayName, isDescending: false)
  {
  }

  public PublishedContentSortOption(PublishedContentSort field, bool isDescending = false) : base(field.ToString(), isDescending)
  {
  }
}
