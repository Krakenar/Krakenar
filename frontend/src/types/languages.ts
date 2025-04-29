import type { Aggregate } from "./aggregate";
import type { Locale } from "./i18n";
import type { Realm } from "./realms";
import type { SearchPayload, SortOption } from "./search";

export type CreateOrReplaceLanguagePayload = {
  locale: string;
};

export type Language = Aggregate & {
  realm?: Realm | null;
  isDefault: boolean;
  locale: Locale;
};

export type LanguageSort = "Code" | "CreatedOn" | "DisplayName" | "EnglishName" | "NativeName" | "UpdatedOn";

export type LanguageSortOption = SortOption & {
  field: LanguageSort;
};

export type SearchLanguagesPayload = SearchPayload & {
  sort: LanguageSortOption[];
};

export type UpdateLanguagePayload = {
  locale?: string;
};
