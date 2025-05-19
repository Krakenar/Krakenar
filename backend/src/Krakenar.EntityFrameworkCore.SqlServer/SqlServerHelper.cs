using Krakenar.EntityFrameworkCore.Relational;
using Logitar.Data;
using Logitar.Data.SqlServer;

namespace Krakenar.EntityFrameworkCore.SqlServer;

public class SqlServerHelper : SqlHelper
{
  public override IDeleteBuilder Delete(TableId table) => new SqlServerDeleteBuilder(table);

  public override IQueryBuilder Query(TableId table) => SqlServerQueryBuilder.From(table);

  public override IUpdateBuilder Update() => new SqlServerUpdateBuilder();
}
