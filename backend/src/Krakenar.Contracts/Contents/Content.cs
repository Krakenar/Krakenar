namespace Krakenar.Contracts.Contents;

public class Content : Aggregate
{
  public ContentType ContentType { get; set; } = new();

  public ContentLocale Invariant { get; set; }
  public List<ContentLocale> Locales { get; set; } = [];

  public Content() : base()
  {
    Invariant = new ContentLocale(this);
  }

  public Content(ContentLocale invariant)
  {
    Invariant = invariant;
  }

  public override string ToString() => $"{Invariant.DisplayName ?? Invariant.UniqueName} | {base.ToString()}";
}
