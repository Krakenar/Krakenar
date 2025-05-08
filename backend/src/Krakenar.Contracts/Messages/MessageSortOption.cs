using Krakenar.Contracts.Search;

namespace Krakenar.Contracts.Messages;

public record MessageSortOption : SortOption
{
  public new MessageSort Field
  {
    get => Enum.Parse<MessageSort>(base.Field);
    set => base.Field = value.ToString();
  }

  public MessageSortOption() : this(MessageSort.UpdatedOn, isDescending: true)
  {
  }

  public MessageSortOption(MessageSort field, bool isDescending = false) : base(field.ToString(), isDescending)
  {
  }
}
