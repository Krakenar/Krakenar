using Krakenar.Contracts.Search;

namespace Krakenar.Contracts.Templates;

public record TemplateSortOption : SortOption
{
  public new TemplateSort Field
  {
    get => Enum.Parse<TemplateSort>(base.Field);
    set => base.Field = value.ToString();
  }

  public TemplateSortOption() : this(TemplateSort.UpdatedOn, isDescending: true)
  {
  }

  public TemplateSortOption(TemplateSort field, bool isDescending = false) : base(field.ToString(), isDescending)
  {
  }
}
