﻿using Krakenar.Contracts.Actors;
using Krakenar.Contracts.Users;

namespace Krakenar.Contracts.Sessions;

public class Session : Aggregate
{
  public User User { get; set; }

  public bool IsPersistent { get; set; }
  public string? RefreshToken { get; set; }

  public bool IsActive { get; set; }
  public Actor? SignedOutBy { get; set; }
  public DateTime? SignedOutOn { get; set; }

  public List<CustomAttribute> CustomAttributes { get; set; } = [];

  public Session() : this(new User())
  {
  }

  public Session(User user)
  {
    User = user;
  }
}
