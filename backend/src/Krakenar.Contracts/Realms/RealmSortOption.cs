using Krakenar.Contracts.Search;

namespace Krakenar.Contracts.Realms;

public record RealmSortOption : SortOption
{
  public new RealmSort Field
  {
    get => Enum.Parse<RealmSort>(base.Field);
    set => base.Field = value.ToString();
  }

  public RealmSortOption() : this(RealmSort.UpdatedOn, isDescending: true)
  {
  }

  public RealmSortOption(RealmSort field, bool isDescending = false) : base(field.ToString(), isDescending)
  {
  }
}
