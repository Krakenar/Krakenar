import type { Actor } from "./actor";
import type { Aggregate } from "./aggregate";
import type { CustomAttribute, CustomIdentifier } from "./custom";
import type { Locale } from "./i18n";
import type { Realm } from "./realms";
import type { Role } from "./roles";
import type { Session } from "./sessions";

export type Address = Contact & {
  street: string;
  locality: string;
  postalCode?: string;
  region?: string;
  country: string;
  formatted: string;
};

export type Contact = {
  isVerified: boolean;
  verifiedBy?: Actor;
  verifiedOn?: string;
};

export type Email = Contact & {
  address: string;
};

export type Phone = Contact & {
  countryCode?: string;
  number: string;
  extension?: string;
  e164Formatted: string;
};

export type User = Aggregate & {
  realm?: Realm;
  uniqueName: string;
  hasPassword: boolean;
  passwordChangedBy?: Actor;
  passwordChangedOn?: string;
  disabledBy?: Actor;
  disabledOn?: string;
  isDisabled: boolean;
  address?: Address;
  email?: Email;
  phone?: Phone;
  isConfirmed: boolean;
  firstName?: string;
  middleName?: string;
  lastName?: string;
  fullName?: string;
  nickname?: string;
  birthdate?: string;
  gender?: string;
  locale?: Locale;
  timeZone?: string;
  picture?: string;
  profile?: string;
  website?: string;
  authenticatedOn?: string;
  customAttributes: CustomAttribute[];
  customIdentifiers: CustomIdentifier[];
  roles: Role[];
  sessions: Session[];
};
