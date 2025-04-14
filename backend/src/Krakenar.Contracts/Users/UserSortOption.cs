using Krakenar.Contracts.Search;

namespace Krakenar.Contracts.Users;

public record UserSortOption : SortOption
{
  public new UserSort Field
  {
    get => Enum.Parse<UserSort>(base.Field);
    set => base.Field = value.ToString();
  }

  public UserSortOption() : this(UserSort.UpdatedOn, isDescending: true)
  {
  }

  public UserSortOption(UserSort field, bool isDescending = false) : base(field.ToString(), isDescending)
  {
  }
}
