import type { Aggregate } from "./aggregate";
import type { Language } from "./languages";
import type { SearchPayload, SortOption } from "./search";

export type CreateOrReplaceDictionaryPayload = {
  language: string;
  entries: DictionaryEntry[];
};

export type Dictionary = Aggregate & {
  language: Language;
  entryCount: number;
  entries: DictionaryEntry[];
};

export type DictionaryEntry = {
  key: string;
  value: string;
  isAdded?: boolean;
  isRemoved?: boolean;
};

export type DictionarySort = "CreatedOn" | "EntryCount" | "Language" | "UpdatedOn";

export type DictionarySortOption = SortOption & {
  field: DictionarySort;
};

export type SearchDictionariesPayload = SearchPayload & {
  sort: DictionarySortOption[];
};

export type UpdateDictionaryPayload = {
  language?: string;
  entries: DictionaryEntry[];
};
