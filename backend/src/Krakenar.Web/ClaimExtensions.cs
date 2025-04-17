using Krakenar.Contracts.Roles;
using Krakenar.Contracts.Sessions;
using Krakenar.Contracts.Users;
using Logitar.Security.Claims;

namespace Krakenar.Web;

public static class ClaimsExtensions
{
  //public static ClaimsIdentity CreateClaimsIdentity(this ApiKeyModel apiKey, string? authenticationType = null)
  //{
  //  ClaimsIdentity identity = new(authenticationType);

  //  identity.AddClaim(new(Rfc7519ClaimNames.Subject, apiKey.Id.ToString()));
  //  identity.AddClaim(ClaimHelper.Create(Rfc7519ClaimNames.UpdatedAt, apiKey.UpdatedOn));
  //  identity.AddClaim(new(Rfc7519ClaimNames.FullName, apiKey.DisplayName));

  //  if (apiKey.AuthenticatedOn.HasValue)
  //  {
  //    identity.AddClaim(ClaimHelper.Create(Rfc7519ClaimNames.AuthenticationTime, apiKey.AuthenticatedOn.Value));
  //  }

  //  return identity;
  //} // ISSUE #15: https://github.com/Krakenar/Krakenar/issues/15

  public static ClaimsIdentity CreateClaimsIdentity(this Session session, string? authenticationType = null)
  {
    ClaimsIdentity identity = session.User.CreateClaimsIdentity(authenticationType);

    identity.AddClaim(new(Rfc7519ClaimNames.SessionId, session.Id.ToString()));

    return identity;
  }
  public static ClaimsIdentity CreateClaimsIdentity(this User user, string? authenticationType = null)
  {
    ClaimsIdentity identity = new(authenticationType);

    identity.AddClaim(new(Rfc7519ClaimNames.Subject, user.Id.ToString()));
    identity.AddClaim(new(Rfc7519ClaimNames.Username, user.UniqueName));
    identity.AddClaim(ClaimHelper.Create(Rfc7519ClaimNames.UpdatedAt, user.UpdatedOn));

    if (user.Address is not null)
    {
      identity.AddClaim(user.Address.CreateClaim(Rfc7519ClaimNames.Address));
      identity.AddClaim(new(OtherClaimNames.IsAddressVerified, user.Address.IsVerified.ToString().ToLower(), ClaimValueTypes.Boolean));
    }
    if (user.Email is not null)
    {
      identity.AddClaim(new(Rfc7519ClaimNames.EmailAddress, user.Email.Address));
      identity.AddClaim(new(Rfc7519ClaimNames.IsEmailVerified, user.Email.IsVerified.ToString().ToLower(), ClaimValueTypes.Boolean));
    }
    if (user.Phone is not null)
    {
      identity.AddClaim(new(Rfc7519ClaimNames.PhoneNumber, user.Phone.E164Formatted));
      identity.AddClaim(new(Rfc7519ClaimNames.IsPhoneVerified, user.Phone.IsVerified.ToString().ToLower(), ClaimValueTypes.Boolean));
    }

    if (user.FullName is not null)
    {
      identity.AddClaim(new(Rfc7519ClaimNames.FullName, user.FullName));

      if (user.FirstName is not null)
      {
        identity.AddClaim(new(Rfc7519ClaimNames.FirstName, user.FirstName));
      }

      if (user.MiddleName is not null)
      {
        identity.AddClaim(new(Rfc7519ClaimNames.MiddleName, user.MiddleName));
      }

      if (user.LastName is not null)
      {
        identity.AddClaim(new(Rfc7519ClaimNames.LastName, user.LastName));
      }
    }
    if (user.Nickname is not null)
    {
      identity.AddClaim(new(Rfc7519ClaimNames.Nickname, user.Nickname));
    }

    if (user.Birthdate.HasValue)
    {
      identity.AddClaim(new(Rfc7519ClaimNames.Birthdate, user.Birthdate.Value.ToString("yyyy-MM-dd")));
    }
    if (user.Gender is not null)
    {
      identity.AddClaim(new(Rfc7519ClaimNames.Gender, user.Gender.ToLower()));
    }
    if (user.Locale is not null)
    {
      identity.AddClaim(new(Rfc7519ClaimNames.Locale, user.Locale.Code));
    }
    if (user.TimeZone is not null)
    {
      identity.AddClaim(new(Rfc7519ClaimNames.TimeZone, user.TimeZone));
    }

    if (user.Picture is not null)
    {
      identity.AddClaim(new(Rfc7519ClaimNames.Picture, user.Picture));
    }
    if (user.Profile is not null)
    {
      identity.AddClaim(new(Rfc7519ClaimNames.Profile, user.Profile));
    }
    if (user.Website is not null)
    {
      identity.AddClaim(new(Rfc7519ClaimNames.Website, user.Website));
    }

    if (user.AuthenticatedOn.HasValue)
    {
      identity.AddClaim(ClaimHelper.Create(Rfc7519ClaimNames.AuthenticationTime, user.AuthenticatedOn.Value));
    }

    foreach (Role role in user.Roles)
    {
      identity.AddClaim(new(Rfc7519ClaimNames.Roles, role.UniqueName));
    }

    return identity;
  }

  private static Claim CreateClaim(this Address address, string name)
  {
    Rfc7519PostalAddress postalAddress = new()
    {
      Formatted = address.Formatted,
      StreetAddress = address.Street,
      Locality = address.Locality,
      Region = address.Region,
      PostalCode = address.PostalCode,
      Country = address.Country
    };

    return new Claim(name, postalAddress.Serialize());
  }
}
