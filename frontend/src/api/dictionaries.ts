import { urlUtils } from "logitar-js";

import type { CreateOrReplaceDictionaryPayload, Dictionary, SearchDictionariesPayload, UpdateDictionaryPayload } from "@/types/dictionaries";
import type { SearchResults } from "@/types/search";
import { _delete, get, patch, post, put } from ".";

function createUrlBuilder(id?: string): urlUtils.IUrlBuilder {
  if (id) {
    return new urlUtils.UrlBuilder({ path: "/api/dictionaries/{id}" }).setParameter("id", id);
  }
  return new urlUtils.UrlBuilder({ path: "/api/dictionaries" });
}

export async function createDictionary(payload: CreateOrReplaceDictionaryPayload): Promise<Dictionary> {
  const url: string = createUrlBuilder().buildRelative();
  return (await post<CreateOrReplaceDictionaryPayload, Dictionary>(url, payload)).data;
}

export async function deleteDictionary(id: string): Promise<Dictionary> {
  const url: string = createUrlBuilder(id).buildRelative();
  return (await _delete<Dictionary>(url)).data;
}

export async function readDictionary(id: string): Promise<Dictionary> {
  const url: string = createUrlBuilder(id).buildRelative();
  return (await get<Dictionary>(url)).data;
}

export async function replaceDictionary(id: string, payload: CreateOrReplaceDictionaryPayload, version?: number): Promise<Dictionary> {
  const url: string = createUrlBuilder(id)
    .setQuery("version", version?.toString() ?? "")
    .buildRelative();
  return (await put<CreateOrReplaceDictionaryPayload, Dictionary>(url, payload)).data;
}

export async function searchDictionaries(payload: SearchDictionariesPayload): Promise<SearchResults<Dictionary>> {
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
  return (await get<SearchResults<Dictionary>>(url)).data;
}

export async function updateDictionary(id: string, payload: UpdateDictionaryPayload): Promise<Dictionary> {
  const url: string = createUrlBuilder(id).buildRelative();
  return (await patch<UpdateDictionaryPayload, Dictionary>(url, payload)).data;
}
