import type { Actor } from "./actor";
import type { Aggregate } from "./aggregate";
import type { Change } from "./change";
import type { FieldDefinition, FieldValue, FieldValuePayload } from "./fields";
import type { Language } from "./languages";
import type { Realm } from "./realms";
import type { SearchPayload, SortOption } from "./search";

export type Content = Aggregate & {
  contentType: ContentType;
  invariant: ContentLocale;
  locales: ContentLocale[];
};

export type ContentLocale = {
  content?: Content | null;
  language?: Language | null;
  uniqueName: string;
  displayName?: string | null;
  description?: string | null;
  fieldValues: FieldValue[];
  version: number;
  createdBy: Actor;
  createdOn: string;
  updatedBy: Actor;
  updatedOn: string;
  isPublished: boolean;
  publishedVersion?: number | null;
  publishedBy?: Actor | null;
  publishedOn?: string | null;
};

export type ContentSort = "CreatedOn" | "DisplayName" | "UniqueName" | "UpdatedOn";

export type ContentSortOption = SortOption & {
  field: ContentSort;
};

export type ContentType = Aggregate & {
  realm?: Realm | null;
  isInvariant: boolean;
  uniqueName: string;
  displayName?: string | null;
  description?: string | null;
  fieldCount: number;
  fields: FieldDefinition[];
};

export type ContentTypeSort = "CreatedOn" | "DisplayName" | "UniqueName" | "UpdatedOn";

export type ContentTypeSortOption = SortOption & {
  field: ContentTypeSort;
};

export type CreateContentPayload = {
  id?: string;
  contentType: string;
  language?: string;
  uniqueName: string;
  displayName?: string;
  description?: string;
  fieldValues: FieldValuePayload[];
};

export type CreateOrReplaceContentTypePayload = {
  isInvariant: boolean;
  uniqueName: string;
  displayName?: string;
  description?: string;
};

export type MediaType = "text/html" | "text/plain";

export type SaveContentLocalePayload = {
  uniqueName: string;
  displayName?: string;
  description?: string;
  fieldValues: FieldValuePayload[];
};

export type SearchContentLocalesPayload = SearchPayload & {
  contentTypeId?: string;
  languageId?: string;
  sort: ContentSortOption[];
};

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
