import { urlUtils } from "logitar-js";

import type { CreateOrReplaceLanguagePayload, Language, SearchLanguagesPayload, UpdateLanguagePayload } from "@/types/languages";
import type { SearchResults } from "@/types/search";
import { _delete, get, patch, post, put } from ".";

function createUrlBuilder(id?: string): urlUtils.IUrlBuilder {
  if (id) {
    return new urlUtils.UrlBuilder({ path: "/api/languages/{id}" }).setParameter("id", id);
  }
  return new urlUtils.UrlBuilder({ path: "/api/languages" });
}

export async function createLanguage(payload: CreateOrReplaceLanguagePayload): Promise<Language> {
  const url: string = createUrlBuilder().buildRelative();
  return (await post<CreateOrReplaceLanguagePayload, Language>(url, payload)).data;
}

export async function deleteLanguage(id: string): Promise<Language> {
  const url: string = createUrlBuilder(id).buildRelative();
  return (await _delete<Language>(url)).data;
}

export async function readLanguage(id: string): Promise<Language> {
  const url: string = createUrlBuilder(id).buildRelative();
  return (await get<Language>(url)).data;
}

export async function replaceLanguage(id: string, payload: CreateOrReplaceLanguagePayload, version?: number): Promise<Language> {
  const url: string = createUrlBuilder(id)
    .setQuery("version", version?.toString() ?? "")
    .buildRelative();
  return (await put<CreateOrReplaceLanguagePayload, Language>(url, payload)).data;
}

export async function searchLanguages(payload: SearchLanguagesPayload): Promise<SearchResults<Language>> {
  const url: string = createUrlBuilder()
    .setQuery("ids", payload.ids)
    .setQuery(
      "search",
      payload.search.terms.map(({ value }) => value),
    )
    .setQuery("search_operator", payload.search.operator)
    .setQuery(
      "sort",
      payload.sort.map(({ field, isDescending }) => (isDescending ? `DESC.${field}` : field)),
    )
    .setQuery("skip", payload.skip.toString())
    .setQuery("limit", payload.limit.toString())
    .buildRelative();
  return (await get<SearchResults<Language>>(url)).data;
}

export async function setDefaultLanguage(id: string): Promise<Language> {
  const url: string = new urlUtils.UrlBuilder({ path: "/api/languages/{id}/default" }).setParameter("id", id).buildRelative();
  return (await patch<void, Language>(url)).data;
}

export async function updateLanguage(id: string, payload: UpdateLanguagePayload): Promise<Language> {
  const url: string = createUrlBuilder(id).buildRelative();
  return (await patch<UpdateLanguagePayload, Language>(url, payload)).data;
}
