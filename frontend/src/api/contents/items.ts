import { urlUtils } from "logitar-js";

import type { Content, ContentLocale, CreateContentPayload, SaveContentLocalePayload, SearchContentLocalesPayload } from "@/types/contents";
import type { SearchResults } from "@/types/search";
import { _delete, get, patch, post, put } from "..";

function createUrlBuilder(id?: string): urlUtils.IUrlBuilder {
  if (id) {
    return new urlUtils.UrlBuilder({ path: "/api/contents/{id}" }).setParameter("id", id);
  }
  return new urlUtils.UrlBuilder({ path: "/api/contents" });
}

export async function createContent(payload: CreateContentPayload): Promise<Content> {
  const url: string = createUrlBuilder().buildRelative();
  return (await post<CreateContentPayload, Content>(url, payload)).data;
}

export async function deleteContent(id: string, language?: string): Promise<Content> {
  const url: string = createUrlBuilder(id)
    .setQuery("language", language ?? "")
    .buildRelative();
  return (await _delete<Content>(url)).data;
}

export async function publishAllContent(id: string): Promise<Content> {
  const url: string = new urlUtils.UrlBuilder({ path: "/api/contents/{id}/publish/all" }).setParameter("id", id).buildRelative();
  return (await patch<void, Content>(url)).data;
}

export async function publishContent(id: string, language?: string): Promise<Content> {
  const url: string = new urlUtils.UrlBuilder({ path: "/api/contents/{id}/publish" })
    .setParameter("id", id)
    .setQuery("language", language ?? "")
    .buildRelative();
  return (await patch<void, Content>(url)).data;
}

export async function readContent(id: string): Promise<Content> {
  const url: string = createUrlBuilder(id).buildRelative();
  return (await get<Content>(url)).data;
}

export async function saveContentLocale(id: string, payload: SaveContentLocalePayload, language?: string): Promise<Content> {
  const url: string = createUrlBuilder(id)
    .setQuery("language", language ?? "")
    .buildRelative();
  return (await put<SaveContentLocalePayload, Content>(url, payload)).data;
}

export async function searchContentLocales(payload: SearchContentLocalesPayload): Promise<SearchResults<ContentLocale>> {
  const url: string = createUrlBuilder()
    .setQuery("ids", payload.ids)
    .setQuery("language", payload.languageId ?? "")
    .setQuery(
      "search",
      payload.search.terms.map(({ value }) => value),
    )
    .setQuery("search_operator", payload.search.operator)
    .setQuery("type", payload.contentTypeId ?? "")
    .setQuery(
      "sort",
      payload.sort.map(({ field, isDescending }) => (isDescending ? `DESC.${field}` : field)),
    )
    .setQuery("skip", payload.skip.toString())
    .setQuery("limit", payload.limit.toString())
    .buildRelative();
  return (await get<SearchResults<ContentLocale>>(url)).data;
}

export async function unpublishAllContent(id: string): Promise<Content> {
  const url: string = new urlUtils.UrlBuilder({ path: "/api/contents/{id}/unpublish/all" }).setParameter("id", id).buildRelative();
  return (await patch<void, Content>(url)).data;
}

export async function unpublishContent(id: string, language?: string): Promise<Content> {
  const url: string = new urlUtils.UrlBuilder({ path: "/api/contents/{id}/unpublish" })
    .setParameter("id", id)
    .setQuery("language", language ?? "")
    .buildRelative();
  return (await patch<void, Content>(url)).data;
}
