namespace Krakenar.EntityFrameworkCore.Relational.Entities;

public sealed class UserRole
{
  public int UserId { get; set; }
  public int RoleId { get; set; }

  public override bool Equals(object? obj) => obj is UserRole entity && entity.UserId == UserId && entity.RoleId == RoleId;
  public override int GetHashCode() => HashCode.Combine(UserId, RoleId);
  public override string ToString() => $"{GetType()} (UserId={UserId}, RoleId={RoleId})";
}
