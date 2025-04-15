namespace Krakenar.Contracts.Users;

public record EmailPayload : ContactPayload, IEmail
{
  public string Address { get; set; }

  public EmailPayload() : this(string.Empty)
  {
  }

  public EmailPayload(string address, bool isVerified = false) : base(isVerified)
  {
    Address = address;
  }
}
