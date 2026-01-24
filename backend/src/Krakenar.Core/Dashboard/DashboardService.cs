using Krakenar.Contracts.Dashboard;
using Krakenar.Core.Dashboard.Queries;
using Logitar.CQRS;

namespace Krakenar.Core.Dashboard;

public class DashboardService : IDashboardService
{
  protected virtual IQueryBus QueryBus { get; }

  public DashboardService(IQueryBus queryBus)
  {
    QueryBus = queryBus;
  }

  public virtual async Task<Statistics> GetStatisticsAsync(CancellationToken cancellationToken)
  {
    GetDashboardStatistics query = new();
    return await QueryBus.ExecuteAsync(query, cancellationToken);
  }
}
