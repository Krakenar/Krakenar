﻿using Krakenar.Contracts.Settings;

namespace Krakenar.Contracts.Realms;

public record CreateOrReplaceRealmPayload
{
  public string UniqueSlug { get; set; } = string.Empty;
  public string? DisplayName { get; set; }
  public string? Description { get; set; }

  public string? Secret { get; set; }
  public string? Url { get; set; }

  public UniqueNameSettings UniqueNameSettings { get; set; } = new();
  public PasswordSettings PasswordSettings { get; set; } = new();
  public bool RequireUniqueEmail { get; set; }
  public bool RequireConfirmedAccount { get; set; }

  public List<CustomAttribute> CustomAttributes { get; set; } = [];
}
