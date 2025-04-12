using Krakenar.EntityFrameworkCore.Relational.Entities;
using Logitar.Data;

namespace Krakenar.EntityFrameworkCore.Relational.KrakenarDb;

public static class Users
{
  public static readonly TableId Table = new(Schemas.Identity, nameof(KrakenarContext.Users), alias: null);

  public static readonly ColumnId CreatedBy = new(nameof(User.CreatedBy), Table);
  public static readonly ColumnId CreatedOn = new(nameof(User.CreatedOn), Table);
  public static readonly ColumnId StreamId = new(nameof(User.StreamId), Table);
  public static readonly ColumnId UpdatedBy = new(nameof(User.UpdatedBy), Table);
  public static readonly ColumnId UpdatedOn = new(nameof(User.UpdatedOn), Table);
  public static readonly ColumnId Version = new(nameof(User.Version), Table);

  public static readonly ColumnId AddressCountry = new(nameof(User.AddressCountry), Table);
  public static readonly ColumnId AddressFormatted = new(nameof(User.AddressFormatted), Table);
  public static readonly ColumnId AddressLocality = new(nameof(User.AddressLocality), Table);
  public static readonly ColumnId AddressPostalCode = new(nameof(User.AddressPostalCode), Table);
  public static readonly ColumnId AddressRegion = new(nameof(User.AddressRegion), Table);
  public static readonly ColumnId AddressStreet = new(nameof(User.AddressStreet), Table);
  public static readonly ColumnId AddressVerifiedBy = new(nameof(User.AddressVerifiedBy), Table);
  public static readonly ColumnId AddressVerifiedOn = new(nameof(User.AddressVerifiedOn), Table);
  public static readonly ColumnId AuthenticatedOn = new(nameof(User.AuthenticatedOn), Table);
  public static readonly ColumnId Birthdate = new(nameof(User.Birthdate), Table);
  public static readonly ColumnId CustomAttributes = new(nameof(User.CustomAttributes), Table);
  public static readonly ColumnId DisabledBy = new(nameof(User.DisabledBy), Table);
  public static readonly ColumnId DisabledOn = new(nameof(User.DisabledOn), Table);
  public static readonly ColumnId EmailAddress = new(nameof(User.EmailAddress), Table);
  public static readonly ColumnId EmailAddressNormalized = new(nameof(User.EmailAddressNormalized), Table);
  public static readonly ColumnId EmailVerifiedBy = new(nameof(User.EmailVerifiedBy), Table);
  public static readonly ColumnId EmailVerifiedOn = new(nameof(User.EmailVerifiedOn), Table);
  public static readonly ColumnId FirstName = new(nameof(User.FirstName), Table);
  public static readonly ColumnId FullName = new(nameof(User.FullName), Table);
  public static readonly ColumnId Gender = new(nameof(User.Gender), Table);
  public static readonly ColumnId HasPassword = new(nameof(User.HasPassword), Table);
  public static readonly ColumnId Id = new(nameof(User.Id), Table);
  public static readonly ColumnId IsAddressVerified = new(nameof(User.IsAddressVerified), Table);
  public static readonly ColumnId IsConfirmed = new(nameof(User.IsConfirmed), Table);
  public static readonly ColumnId IsDisabled = new(nameof(User.IsDisabled), Table);
  public static readonly ColumnId IsEmailVerified = new(nameof(User.IsEmailVerified), Table);
  public static readonly ColumnId IsPhoneVerified = new(nameof(User.IsPhoneVerified), Table);
  public static readonly ColumnId LastName = new(nameof(User.LastName), Table);
  public static readonly ColumnId Locale = new(nameof(User.Locale), Table);
  public static readonly ColumnId MiddleName = new(nameof(User.MiddleName), Table);
  public static readonly ColumnId Nickname = new(nameof(User.Nickname), Table);
  public static readonly ColumnId PasswordChangedBy = new(nameof(User.PasswordChangedBy), Table);
  public static readonly ColumnId PasswordChangedOn = new(nameof(User.PasswordChangedOn), Table);
  public static readonly ColumnId PasswordHash = new(nameof(User.PasswordHash), Table);
  public static readonly ColumnId PhoneCountryCode = new(nameof(User.PhoneCountryCode), Table);
  public static readonly ColumnId PhoneE164Formatted = new(nameof(User.PhoneE164Formatted), Table);
  public static readonly ColumnId PhoneExtension = new(nameof(User.PhoneExtension), Table);
  public static readonly ColumnId PhoneNumber = new(nameof(User.PhoneNumber), Table);
  public static readonly ColumnId PhoneVerifiedBy = new(nameof(User.PhoneVerifiedBy), Table);
  public static readonly ColumnId PhoneVerifiedOn = new(nameof(User.PhoneVerifiedOn), Table);
  public static readonly ColumnId Picture = new(nameof(User.Picture), Table);
  public static readonly ColumnId Profile = new(nameof(User.Profile), Table);
  public static readonly ColumnId RealmId = new(nameof(User.RealmId), Table);
  public static readonly ColumnId RealmUid = new(nameof(User.RealmUid), Table);
  public static readonly ColumnId TimeZone = new(nameof(User.TimeZone), Table);
  public static readonly ColumnId UniqueName = new(nameof(User.UniqueName), Table);
  public static readonly ColumnId UniqueNameNormalized = new(nameof(User.UniqueNameNormalized), Table);
  public static readonly ColumnId UserId = new(nameof(User.UserId), Table);
  public static readonly ColumnId Website = new(nameof(User.Website), Table);
}
