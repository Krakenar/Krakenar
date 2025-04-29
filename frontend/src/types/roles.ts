import type { Aggregate } from "./aggregate";
import type { Change } from "./change";
import type { CustomAttribute } from "./custom";
import type { Realm } from "./realms";
import type { SearchPayload, SortOption } from "./search";

export type CollectionAction = "Add" | "Remove";

export type CreateOrReplaceRolePayload = {
  uniqueName: string;
  displayName?: string;
  description?: string;
  customAttributes: CustomAttribute[];
};

export type Role = Aggregate & {
  realm?: Realm | null;
  uniqueName: string;
  displayName?: string | null;
  description?: string | null;
  customAttributes: CustomAttribute[];
};

export type RoleChange = {
  role: string;
  action: CollectionAction;
};

export type RoleSort = "CreatedOn" | "DisplayName" | "UniqueName" | "UpdatedOn";

export type RoleSortOption = SortOption & {
  field: RoleSort;
};

export type SearchRolesPayload = SearchPayload & {
  sort: RoleSortOption[];
};

export type UpdateRolePayload = {
  uniqueName?: string;
  displayName?: Change<string>;
  description?: Change<string>;
  customAttributes: CustomAttribute[];
};
