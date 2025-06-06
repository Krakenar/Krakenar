import type { Actor } from "./actor";
import type { Aggregate } from "./aggregate";
import type { Change } from "./change";
import type { MediaType } from "./contents";
import type { Realm } from "./realms";
import type { SearchPayload, SortOption } from "./search";

export type BooleanSettings = {};

export type CreateOrReplaceFieldDefinitionPayload = {
  fieldType: string;
  isInvariant: boolean;
  isRequired: boolean;
  isIndexed: boolean;
  isUnique: boolean;
  uniqueName: string;
  displayName?: string;
  description?: string;
  placeholder?: string;
};

export type CreateOrReplaceFieldTypePayload = {
  uniqueName: string;
  displayName?: string;
  description?: string;
  boolean?: BooleanSettings;
  dateTime?: DateTimeSettings;
  number?: NumberSettings;
  relatedContent?: RelatedContentSettings;
  richText?: RichTextSettings;
  select?: SelectSettings;
  string?: StringSettings;
  tags?: TagsSettings;
};

export type DataType = "Boolean" | "DateTime" | "Number" | "RelatedContent" | "RichText" | "Select" | "String" | "Tags";

export type DateTimeSettings = {
  minimumValue?: string | null;
  maximumValue?: string | null;
};

export type FieldDefinition = {
  id: string;
  order: number;
  fieldType: FieldType;
  isInvariant: boolean;
  isRequired: boolean;
  isIndexed: boolean;
  isUnique: boolean;
  uniqueName: string;
  displayName?: string | null;
  description?: string | null;
  placeholder?: string | null;
  createdBy: Actor;
  createdOn: string;
  updatedBy: Actor;
  updatedOn: string;
};

export type FieldType = Aggregate & {
  realm?: Realm | null;
  uniqueName: string;
  displayName?: string | null;
  description?: string | null;
  dataType: DataType;
  boolean?: BooleanSettings | null;
  dateTime?: DateTimeSettings | null;
  number?: NumberSettings | null;
  relatedContent?: RelatedContentSettings | null;
  richText?: RichTextSettings | null;
  select?: SelectSettings | null;
  string?: StringSettings | null;
  tags?: TagsSettings | null;
};

export type FieldTypeSort = "CreatedOn" | "DisplayName" | "UniqueName" | "UpdatedOn";

export type FieldTypeSortOption = SortOption & {
  field: FieldTypeSort;
};

export type FieldValue = {
  id: string;
  value: string;
};

export type FieldValuePayload = {
  field: string;
  value: string;
};

export type NumberSettings = {
  minimumValue?: number | null;
  maximumValue?: number | null;
  step?: number | null;
};

export type RelatedContentSettings = {
  contentTypeId: string;
  isMultiple: boolean;
};

export type RichTextSettings = {
  contentType: MediaType;
  minimumLength?: number | null;
  maximumLength?: number | null;
};

export type SearchFieldTypesPayload = SearchPayload & {
  dataType?: DataType;
  sort: FieldTypeSortOption[];
};

export type SelectOption = {
  text: string;
  value?: string | null;
  label?: string | null;
  isDisabled: boolean;
};

export type SelectSettings = {
  options: SelectOption[];
  isMultiple: boolean;
};

export type StringSettings = {
  minimumLength?: number | null;
  maximumLength?: number | null;
  pattern?: string | null;
};

export type SwitchFieldDefinitionsPayload = {
  fields: string[];
};

export type TagsSettings = {};

export type UpdateFieldDefinitionPayload = {
  isInvariant?: boolean;
  isRequired?: boolean;
  isIndexed?: boolean;
  isUnique?: boolean;
  uniqueName?: string;
  displayName?: Change<string>;
  description?: Change<string>;
  placeholder?: Change<string>;
};

export type UpdateFieldTypePayload = {
  uniqueName?: string;
  displayName?: Change<string>;
  description?: Change<string>;
  boolean?: BooleanSettings;
  dateTime?: DateTimeSettings;
  number?: NumberSettings;
  relatedContent?: RelatedContentSettings;
  richText?: RichTextSettings;
  select?: SelectSettings;
  string?: StringSettings;
  tags?: TagsSettings;
};
