namespace Krakenar.Contracts.Actors;

public class Actor
{
  public Guid Id { get; set; }
  public ActorType Type { get; set; }
  public bool IsDeleted { get; set; }

  public string DisplayName { get; set; } = "System";
  public string? EmailAddress { get; set; }
  public string? PictureUrl { get; set; }

  public Actor()
  {
  }

  public override bool Equals(object obj) => obj is Actor actor && actor.Id == Id;
  public override int GetHashCode() => Id.GetHashCode();
  public override string ToString()
  {
    StringBuilder actor = new();
    actor.Append(DisplayName);
    if (EmailAddress is not null)
    {
      actor.Append(" <").Append(EmailAddress).Append('>');
    }
    actor.Append(" (").Append(Type).Append(".Id=").Append(Id).Append(')');
    return actor.ToString();
  }
}
