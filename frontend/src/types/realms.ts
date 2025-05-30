import type { Actor } from "./actor";
import type { Aggregate } from "./aggregate";
import type { Change } from "./change";
import type { CustomAttribute } from "./custom";
import type { PasswordSettings, UniqueNameSettings } from "./settings";
import type { SearchPayload, SortOption } from "./search";

export type CreateOrReplaceRealmPayload = {
  uniqueSlug: string;
  displayName?: string;
  description?: string;
  url?: string;
  uniqueNameSettings: UniqueNameSettings;
  passwordSettings: PasswordSettings;
  requireUniqueEmail: boolean;
  requireConfirmedAccount: boolean;
  customAttributes: CustomAttribute[];
};

export type Realm = Aggregate & {
  uniqueSlug: string;
  displayName?: string | null;
  description?: string | null;
  secretChangedBy: Actor;
  secretChangedOn: string;
  url?: string | null;
  uniqueNameSettings: UniqueNameSettings;
  passwordSettings: PasswordSettings;
  requireUniqueEmail: boolean;
  requireConfirmedAccount: boolean;
  customAttributes: CustomAttribute[];
};

export type RealmSort = "CreatedOn" | "DisplayName" | "SecretChangedOn" | "UniqueSlug" | "UpdatedOn";

export type RealmSortOption = SortOption & {
  field: RealmSort;
};

export type SearchRealmsPayload = SearchPayload & {
  sort: RealmSortOption[];
};

export type UpdateRealmPayload = {
  uniqueSlug?: string;
  displayName?: Change<string>;
  description?: Change<string>;
  secret?: Change<string>;
  url?: Change<string>;
  uniqueNameSettings?: UniqueNameSettings;
  passwordSettings?: PasswordSettings;
  requireUniqueEmail?: boolean;
  requireConfirmedAccount?: boolean;
  customAttributes: CustomAttribute[];
};
