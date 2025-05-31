namespace Krakenar.Contracts.Dashboard;

public interface IDashboardService
{
  Task<Statistics> GetStatisticsAsync(CancellationToken cancellationToken = default);
}
