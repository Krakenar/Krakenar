namespace Krakenar.EntityFrameworkCore.Relational.Entities;

public sealed class ApiKeyRole
{
  public int ApiKeyId { get; set; }
  public int RoleId { get; set; }

  public override bool Equals(object? obj) => obj is ApiKeyRole entity && entity.ApiKeyId == ApiKeyId && entity.RoleId == RoleId;
  public override int GetHashCode() => HashCode.Combine(ApiKeyId, RoleId);
  public override string ToString() => $"{GetType()} (ApiKeyId={ApiKeyId}, RoleId={RoleId})";
}
