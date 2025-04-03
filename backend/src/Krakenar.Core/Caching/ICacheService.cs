using Krakenar.Contracts.Configurations;

namespace Krakenar.Core.Caching;

public interface ICacheService
{
  ConfigurationModel? Configuration { get; set; }
}
