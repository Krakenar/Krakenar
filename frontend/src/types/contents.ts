import type { Aggregate } from "./aggregate";
import type { Change } from "./change";
import type { Realm } from "./realms";
import type { SearchPayload, SortOption } from "./search";

export type ContentType = Aggregate & {
  realm?: Realm | null;
  isInvariant: boolean;
  uniqueName: string;
  displayName?: string | null;
  description?: string | null;
};

export type ContentTypeSort = "CreatedOn" | "DisplayName" | "UniqueName" | "UpdatedOn";

export type ContentTypeSortOption = SortOption & {
  field: ContentTypeSort;
};

export type CreateOrReplaceContentTypePayload = {
  isInvariant: boolean;
  uniqueName: string;
  displayName?: string;
  description?: string;
};

export type MediaType = "text/html" | "text/plain";

export type SearchContentTypesPayload = SearchPayload & {
  isInvariant?: boolean;
  sort: ContentTypeSortOption[];
};

export type UpdateContentTypePayload = {
  isInvariant?: boolean;
  uniqueName?: string;
  displayName?: Change<string>;
  description?: Change<string>;
};
