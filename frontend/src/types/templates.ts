import type { Aggregate } from "./aggregate";
import type { Change } from "./change";
import type { MediaType } from "./contents";
import type { Realm } from "./realms";
import type { SearchPayload, SortOption } from "./search";

export type Content = {
  type: MediaType;
  text: string;
};

export type CreateOrReplaceTemplatePayload = {
  uniqueName: string;
  displayName?: string;
  description?: string;
  subject: string;
  content: Content;
};

export type SearchTemplatesPayload = SearchPayload & {
  contentType?: MediaType;
  sort: TemplateSortOption[];
};

export type Template = Aggregate & {
  realm?: Realm | null;
  uniqueName: string;
  displayName?: string | null;
  description?: string | null;
  subject: string;
  content: Content;
};

export type TemplateSort = "CreatedOn" | "DisplayName" | "Subject" | "UniqueName" | "UpdatedOn";

export type TemplateSortOption = SortOption & {
  field: TemplateSort;
};

export type UpdateTemplatePayload = {
  uniqueName?: string;
  displayName?: Change<string>;
  description?: Change<string>;
  subject?: string;
  content?: Content;
};
