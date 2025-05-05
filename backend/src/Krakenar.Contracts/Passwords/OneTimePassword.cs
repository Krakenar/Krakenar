using Krakenar.Contracts.Realms;
using Krakenar.Contracts.Users;

namespace Krakenar.Contracts.Passwords;

public class OneTimePassword : Aggregate
{
  public Realm? Realm { get; set; }
  public User? User { get; set; }

  public string? Password { get; set; }

  public DateTime? ExpiresOn { get; set; }
  public int? MaximumAttempts { get; set; }

  public int AttemptCount { get; set; }
  public bool HasValidationSucceeded { get; set; }

  public List<CustomAttribute> CustomAttributes { get; set; } = [];
}
