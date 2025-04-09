﻿using Krakenar.Contracts.Settings;

namespace Krakenar.Contracts.Realms;

public record UpdateRealmPayload
{
  public string? UniqueSlug { get; set; }
  public Change<string>? DisplayName { get; set; }
  public Change<string>? Description { get; set; }

  public Change<string>? Url { get; set; }

  public UniqueNameSettings? UniqueNameSettings { get; set; } = new();
  public PasswordSettings? PasswordSettings { get; set; } = new();
  public bool? RequireUniqueEmail { get; set; }
  public bool? RequireConfirmedAccount { get; set; }

  public List<CustomAttribute> CustomAttributes { get; set; } = [];
}
