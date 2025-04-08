using Logitar.Data;
using ConfigurationEntity = Krakenar.EntityFrameworkCore.Relational.Entities.Configuration;

namespace Krakenar.EntityFrameworkCore.Relational.KrakenarDb;

public static class Configuration
{
  public static readonly TableId Table = new(Schemas.Identity, nameof(KrakenarContext.Configuration), alias: null);

  public static readonly ColumnId ConfigurationId = new(nameof(ConfigurationEntity.ConfigurationId), Table);
  public static readonly ColumnId CreatedBy = new(nameof(ConfigurationEntity.CreatedBy), Table);
  public static readonly ColumnId CreatedOn = new(nameof(ConfigurationEntity.CreatedOn), Table);
  public static readonly ColumnId Key = new(nameof(ConfigurationEntity.Key), Table);
  public static readonly ColumnId UpdatedBy = new(nameof(ConfigurationEntity.UpdatedBy), Table);
  public static readonly ColumnId UpdatedOn = new(nameof(ConfigurationEntity.UpdatedOn), Table);
  public static readonly ColumnId Value = new(nameof(ConfigurationEntity.Value), Table);
  public static readonly ColumnId Version = new(nameof(ConfigurationEntity.Version), Table);
}
