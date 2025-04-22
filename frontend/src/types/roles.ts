import type { Aggregate } from "./aggregate";
import type { Change } from "./change";
import type { CustomAttribute } from "./custom";
import type { Realm } from "./realms";
import type { SearchPayload, SortOption } from "./search";

export type CreateOrReplaceRolePayload = {
  uniqueName: string;
  displayName?: string | null;
  description?: string | null;
  customAttributes: CustomAttribute[];
};

export type Role = Aggregate & {
  realm?: Realm | null;
  uniqueName: string;
  displayName?: string | null;
  description?: string | null;
  customAttributes: CustomAttribute[];
};

export type RoleSort = "CreatedOn" | "DisplayName" | "UniqueName" | "UpdatedOn";

export type RoleSortOption = SortOption & {
  field: RoleSort;
};

export type SearchRolesPayload = SearchPayload & {
  sort: RoleSortOption[];
};

export type UpdateRolePayload = {
  uniqueName?: string | null;
  displayName?: Change<string> | null;
  description?: Change<string> | null;
  customAttributes: CustomAttribute[];
};
