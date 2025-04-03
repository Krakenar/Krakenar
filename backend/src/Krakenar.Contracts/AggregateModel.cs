using Krakenar.Contracts.Actors;

namespace Krakenar.Contracts;

public abstract class AggregateModel
{
  public Guid Id { get; set; }
  public long Version { get; set; }

  public Actor CreatedBy { get; set; } = new();
  public DateTime CreatedOn { get; set; }

  public Actor UpdatedBy { get; set; } = new();
  public DateTime UpdatedOn { get; set; }

  public override bool Equals(object obj) => obj is AggregateModel aggregate && aggregate.Id == Id;
  public override int GetHashCode() => Id.GetHashCode();
  public override string ToString() => $"{GetType()} (Id={Id})";
}
