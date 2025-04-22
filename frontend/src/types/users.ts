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
  postalCode?: string | null;
  region?: string | null;
  country: string;
  formatted: string;
};

export type Contact = {
  isVerified: boolean;
  verifiedBy?: Actor | null;
  verifiedOn?: string | null;
};

export type Email = Contact & {
  address: string;
};

export type Phone = Contact & {
  countryCode?: string | null;
  number: string;
  extension?: string | null;
  e164Formatted: string;
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
