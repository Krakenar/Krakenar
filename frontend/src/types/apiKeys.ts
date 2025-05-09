import type { Aggregate } from "./aggregate";
import type { Change } from "./change";
import type { CustomAttribute } from "./custom";
import type { Realm } from "./realms";
import type { Role, RoleChange } from "./roles";
import type { SearchPayload, SortOption } from "./search";

export type ApiKey = Aggregate & {
  realm?: Realm | null;
  xApiKey?: string | null;
  name: string;
  description?: string | null;
  expiresOn?: string | null;
  authenticatedOn?: string | null;
  customAttributes: CustomAttribute[];
  roles: Role[];
};

export type ApiKeySort = "AuthenticatedOn" | "CreatedOn" | "ExpiresOn" | "Name" | "UpdatedOn";

export type ApiKeySortOption = SortOption & {
  field: ApiKeySort;
};

export type ApiKeyStatus = {
  isExpired: boolean;
  moment?: Date;
};

export type CreateOrReplaceApiKeyPayload = {
  name: string;
  description?: string;
  expiresOn?: Date;
  customAttributes: CustomAttribute[];
  roles: string[];
};

export type SearchApiKeysPayload = SearchPayload & {
  hasAuthenticated?: boolean;
  roleId?: string;
  status?: ApiKeyStatus;
  sort: ApiKeySortOption[];
};

export type UpdateApiKeyPayload = {
  name?: string;
  description?: Change<string>;
  expiresOn?: Date;
  customAttributes: CustomAttribute[];
  roles: RoleChange[];
};
