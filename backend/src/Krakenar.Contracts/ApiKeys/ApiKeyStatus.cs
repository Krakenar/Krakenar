namespace Krakenar.Contracts.ApiKeys;

public record ApiKeyStatus
{
  public bool IsExpired { get; set; }
  public DateTime? Moment { get; set; }

  public ApiKeyStatus()
  {
  }

  public ApiKeyStatus(bool isExpired, DateTime? moment = null)
  {
    IsExpired = isExpired;
    Moment = moment;
  }
}
