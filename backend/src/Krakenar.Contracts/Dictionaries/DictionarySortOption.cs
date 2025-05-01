using Krakenar.Contracts.Search;

namespace Krakenar.Contracts.Dictionaries;

public record DictionarySortOption : SortOption
{
  public new DictionarySort Field
  {
    get => Enum.Parse<DictionarySort>(base.Field);
    set => base.Field = value.ToString();
  }

  public DictionarySortOption() : this(DictionarySort.UpdatedOn, isDescending: true)
  {
  }

  public DictionarySortOption(DictionarySort field, bool isDescending = false) : base(field.ToString(), isDescending)
  {
  }
}
