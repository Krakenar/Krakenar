using Krakenar.Contracts.Search;

namespace Krakenar.Contracts.Fields;

public record FieldTypeSortOption : SortOption
{
  public new FieldTypeSort Field
  {
    get => Enum.Parse<FieldTypeSort>(base.Field);
    set => base.Field = value.ToString();
  }

  public FieldTypeSortOption() : this(FieldTypeSort.UpdatedOn, isDescending: true)
  {
  }

  public FieldTypeSortOption(FieldTypeSort field, bool isDescending = false) : base(field.ToString(), isDescending)
  {
  }
}
