import type { Actor } from "./actor";
import type { Aggregate } from "./aggregate";
import type { Change } from "./change";
import type { CustomAttribute, CustomIdentifier } from "./custom";
import type { Locale } from "./i18n";
import type { Realm } from "./realms";
import type { Role, RoleChange } from "./roles";
import type { SearchPayload, SortOption } from "./search";
import type { Session } from "./sessions";

export type Address = Contact & {
  street: string;
  locality: string;
  postalCode?: string | null;
  region?: string | null;
  country: string;
  formatted: string;
};

export type AddressPayload = ContactPayload & {
  street: string;
  locality: string;
  postalCode?: string;
  region?: string;
  country: string;
};

export type ChangePasswordPayload = {
  current?: string;
  new: string;
};

export type Contact = {
  isVerified: boolean;
  verifiedBy?: Actor | null;
  verifiedOn?: string | null;
};

export type ContactPayload = {
  isVerified: boolean;
};

export type Country = {
  code: string;
  name: string;
  postalCode?: string;
  regions?: string[];
  flags: CountryFlags;
};

export type CountryFlags = {
  png: string;
  svg: string;
  alt: string;
};

export type CreateOrReplaceUserPayload = {
  uniqueName: string;
  password?: ChangePasswordPayload;
  isDisabled?: boolean;
  address?: AddressPayload;
  email?: EmailPayload;
  phone?: PhonePayload;
  firstName?: string;
  middleName?: string;
  lastName?: string;
  nickname?: string;
  birthdate?: Date;
  gender?: string;
  locale?: string;
  timeZone?: string;
  picture?: string;
  profile?: string;
  website?: string;
  customAttributes: CustomAttribute[];
  roles: string[];
};

export type Email = Contact & {
  address: string;
};

export type EmailPayload = ContactPayload & {
  address: string;
};

export type PersonNameKind = "first" | "middle" | "last" | "nick";

export type Phone = Contact & {
  countryCode?: string | null;
  number: string;
  extension?: string | null;
  e164Formatted: string;
};

export type PhonePayload = ContactPayload & {
  countryCode?: string;
  number: string;
  extension?: string;
};

export type SaveUserIdentifierPayload = { value: string };

export type SearchUsersPayload = SearchPayload & {
  hasPassword?: boolean | null;
  isDisabled?: boolean | null;
  isConfirmed?: boolean | null;
  hasAuthenticated?: boolean | null;
  roleId?: string | null;
  sort: UserSortOption[];
};

export type UpdateUserPayload = {
  uniqueName?: string;
  password?: ChangePasswordPayload;
  isDisabled?: boolean;
  address?: Change<AddressPayload>;
  email?: Change<EmailPayload>;
  phone?: Change<PhonePayload>;
  firstName?: Change<string>;
  middleName?: Change<string>;
  lastName?: Change<string>;
  nickname?: Change<string>;
  birthdate?: Change<Date>;
  gender?: Change<string>;
  locale?: Change<string>;
  timeZone?: Change<string>;
  picture?: Change<string>;
  profile?: Change<string>;
  website?: Change<string>;
  customAttributes: CustomAttribute[];
  roles: RoleChange[];
};

export type User = Aggregate & {
  realm?: Realm | null;
  uniqueName: string;
  hasPassword: boolean;
  passwordChangedBy?: Actor | null;
  passwordChangedOn?: string | null;
  disabledBy?: Actor | null;
  disabledOn?: string | null;
  isDisabled: boolean;
  address?: Address | null;
  email?: Email | null;
  phone?: Phone | null;
  isConfirmed: boolean;
  firstName?: string | null;
  middleName?: string | null;
  lastName?: string | null;
  fullName?: string | null;
  nickname?: string | null;
  birthdate?: string | null;
  gender?: string | null;
  locale?: Locale | null;
  timeZone?: string | null;
  picture?: string | null;
  profile?: string | null;
  website?: string | null;
  authenticatedOn?: string | null;
  customAttributes: CustomAttribute[];
  customIdentifiers: CustomIdentifier[];
  roles: Role[];
  sessions: Session[];
};

export type UserSort =
  | "AuthenticatedOn"
  | "Birthdate"
  | "CreatedOn"
  | "DisabledOn"
  | "EmailAddress"
  | "FirstName"
  | "FullName"
  | "LastName"
  | "MiddleName"
  | "Nickname"
  | "PasswordChangedOn"
  | "PhoneNumber"
  | "UniqueName"
  | "UpdatedOn";

export type UserSortOption = SortOption & {
  field: UserSort;
};
