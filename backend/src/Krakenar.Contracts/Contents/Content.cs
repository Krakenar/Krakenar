namespace Krakenar.Contracts.Contents;

public class Content : Aggregate
{
  public ContentType ContentType { get; set; } = new();

  public ContentLocale Invariant { get; set; } = new();
  public List<ContentLocale> Locales { get; set; } = [];

  public override string ToString() => $"{Invariant.DisplayName ?? Invariant.UniqueName} | {base.ToString()}";
}
