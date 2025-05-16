using Krakenar.Contracts.Actors;

namespace Krakenar.Contracts.Fields;

public class FieldDefinition
{
  public Guid Id { get; set; }

  public int Order { get; set; }

  public FieldType FieldType { get; set; } = new();

  public bool IsInvariant { get; set; }
  public bool IsRequired { get; set; }
  public bool IsIndexed { get; set; }
  public bool IsUnique { get; set; }

  public string UniqueName { get; set; }
  public string? DisplayName { get; set; }
  public string? Description { get; set; }
  public string? Placeholder { get; set; }

  public Actor CreatedBy { get; set; } = new();
  public DateTime CreatedOn { get; set; }

  public Actor UpdatedBy { get; set; } = new();
  public DateTime UpdatedOn { get; set; }

  public FieldDefinition() : this(string.Empty)
  {
  }

  public FieldDefinition(string uniqueName)
  {
    UniqueName = uniqueName;
  }

  public override bool Equals(object obj) => obj is FieldDefinition field && field.Id == Id;
  public override int GetHashCode() => Id.GetHashCode();
  public override string ToString() => $"{DisplayName ?? UniqueName} | {GetType()} (Id={Id})";
}
