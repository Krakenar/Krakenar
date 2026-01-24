using Krakenar.Contracts.Dashboard;
using Logitar.CQRS;

namespace Krakenar.Core.Dashboard.Queries;

public record GetDashboardStatistics : IQuery<Statistics>;
