using Krakenar.Contracts.Search;
using Logitar.Data;

namespace Krakenar.EntityFrameworkCore.Relational;

public interface ISqlHelper
{
  IQueryBuilder ApplyTextSearch(IQueryBuilder builder, TextSearch search, params ColumnId[] columns);

  IQueryBuilder Query(TableId table);
}

public abstract class SqlHelper : ISqlHelper
{
  public virtual IQueryBuilder ApplyTextSearch(IQueryBuilder builder, TextSearch search, params ColumnId[] columns)
  {
    int termCount = search.Terms.Count;
    if (termCount == 0 || columns.Length == 0)
    {
      return builder;
    }

    List<Condition> conditions = new(capacity: termCount);
    foreach (SearchTerm term in search.Terms)
    {
      if (!string.IsNullOrWhiteSpace(term.Value))
      {
        string pattern = term.Value.Trim();
        conditions.Add(columns.Length == 1
          ? new OperatorCondition(columns.Single(), CreateOperator(pattern))
          : new OrCondition(columns.Select(column => new OperatorCondition(column, CreateOperator(pattern))).ToArray()));
      }
    }

    if (conditions.Count > 0)
    {
      switch (search.Operator)
      {
        case SearchOperator.And:
          return builder.WhereAnd([.. conditions]);
        case SearchOperator.Or:
          return builder.WhereOr([.. conditions]);
      }
    }

    return builder;
  }
  protected virtual ConditionalOperator CreateOperator(string pattern) => Operators.IsLike(pattern);

  public abstract IQueryBuilder Query(TableId table);
}
