namespace Krakenar.Contracts.Users;

public record Email : Contact, IEmail
{
  public string Address { get; set; }

  public Email() : this(string.Empty)
  {
  }

  public Email(IEmail email) : this(email.Address, email.IsVerified)
  {
    IsVerified = email.IsVerified;
  }

  public Email(string address, bool isVerified = false) : base(isVerified)
  {
    Address = address;
  }
}
