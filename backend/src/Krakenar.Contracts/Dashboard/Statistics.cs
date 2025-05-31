using Krakenar.Contracts.Sessions;

namespace Krakenar.Contracts.Dashboard;

public record Statistics
{
  public int RealmCount { get; set; }
  public int UserCount { get; set; }
  public int SessionCount { get; set; }
  public int MessageCount { get; set; }
  public int ContentCount { get; set; }

  public List<Session> Sessions { get; set; } = [];
}
