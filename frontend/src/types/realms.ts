import type { Aggregate } from "./aggregate";
import type { Change } from "./change";
import type { CustomAttribute } from "./custom";
import type { PasswordSettings, UniqueNameSettings } from "./settings";
import type { SearchPayload, SortOption } from "./search";

export type CreateOrReplaceRealmPayload = {
  uniqueSlug: string;
  displayName?: string | null;
  description?: string | null;
  url?: string | null;
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
  url?: string | null;
  uniqueNameSettings: UniqueNameSettings;
  passwordSettings: PasswordSettings;
  requireUniqueEmail: boolean;
  requireConfirmedAccount: boolean;
  customAttributes: CustomAttribute[];
};

export type RealmSort = "CreatedOn" | "DisplayName" | "UniqueSlug" | "UpdatedOn";

export type RealmSortOption = SortOption & {
  field: RealmSort;
};

export type SearchRealmsPayload = SearchPayload & {
  sort: RealmSortOption[];
};

export type UpdateRealmPayload = {
  uniqueSlug?: string | null;
  displayName?: Change<string> | null;
  description?: Change<string> | null;
  url?: Change<string> | null;
  uniqueNameSettings?: UniqueNameSettings | null;
  passwordSettings?: PasswordSettings | null;
  requireUniqueEmail?: boolean | null;
  requireConfirmedAccount?: boolean | null;
  customAttributes: CustomAttribute[];
};
