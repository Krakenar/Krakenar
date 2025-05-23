import { urlUtils } from "logitar-js";

import type { Content, ContentLocale, CreateContentPayload, SearchContentLocalesPayload } from "@/types/contents";
import type { SearchResults } from "@/types/search";
import { get, post } from "..";

function createUrlBuilder(): urlUtils.IUrlBuilder {
  return new urlUtils.UrlBuilder({ path: "/api/contents" });
}

export async function createContent(payload: CreateContentPayload): Promise<Content> {
  const url: string = createUrlBuilder().buildRelative();
  return (await post<CreateContentPayload, Content>(url, payload)).data;
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
