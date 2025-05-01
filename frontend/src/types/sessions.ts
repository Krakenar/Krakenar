import type { Actor } from "./actor";
import type { Aggregate } from "./aggregate";
import type { CustomAttribute } from "./custom";
import type { SearchPayload, SortOption } from "./search";
import type { User } from "./users";

export type SearchSessionsPayload = SearchPayload & {
  userId?: string;
  isActive?: boolean;
  isPersistent?: boolean;
  sort: SessionSortOption[];
};

export type Session = Aggregate & {
  user: User;
  isPersistent: boolean;
  refreshToken?: string | null;
  isActive: boolean;
  signedOutBy?: Actor | null;
  signedOutOn?: string | null;
  customAttributes: CustomAttribute[];
};

export type SessionSort = "CreatedOn" | "SignedOutOn" | "UpdatedOn";

export type SessionSortOption = SortOption & {
  field: SessionSort;
};
