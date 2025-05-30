﻿using Krakenar.Contracts.Realms;
using Krakenar.Contracts.Roles;

namespace Krakenar.Contracts.ApiKeys;

public class ApiKey : Aggregate
{
  public Realm? Realm { get; set; }

  public string? XApiKey { get; set; }

  public string Name { get; set; }
  public string? Description { get; set; }
  public DateTime? ExpiresOn { get; set; }

  public DateTime? AuthenticatedOn { get; set; }

  public List<CustomAttribute> CustomAttributes { get; set; } = [];
  public List<Role> Roles { get; set; } = [];

  public ApiKey() : this(string.Empty)
  {
  }

  public ApiKey(string name)
  {
    Name = name;
  }

  public override string ToString() => $"{Name} | {base.ToString()}";
}
