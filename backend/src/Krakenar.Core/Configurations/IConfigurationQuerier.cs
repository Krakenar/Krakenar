﻿using ConfigurationDto = Krakenar.Contracts.Configurations.Configuration;

namespace Krakenar.Core.Configurations;

public interface IConfigurationQuerier
{
  Task<ConfigurationDto> ReadAsync(Configuration configuration, CancellationToken cancellationToken = default);
}
