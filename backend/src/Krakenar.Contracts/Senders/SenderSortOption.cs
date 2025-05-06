using Krakenar.Contracts.Search;

namespace Krakenar.Contracts.Senders;

public record SenderSortOption : SortOption
{
  public new SenderSort Field
  {
    get => Enum.Parse<SenderSort>(base.Field);
    set => base.Field = value.ToString();
  }

  public SenderSortOption() : this(SenderSort.UpdatedOn, isDescending: true)
  {
  }

  public SenderSortOption(SenderSort field, bool isDescending = false) : base(field.ToString(), isDescending)
  {
  }
}
