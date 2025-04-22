import { urlUtils } from "logitar-js";

import type { CreateOrReplaceRealmPayload, Realm, SearchRealmsPayload, UpdateRealmPayload } from "@/types/realms";
import type { SearchResults } from "@/types/search";
import { get, patch, post, put } from ".";

function createUrlBuilder(id?: string): urlUtils.IUrlBuilder {
  if (id) {
    return new urlUtils.UrlBuilder({ path: "/api/realms/{id}" }).setParameter("id", id);
  }
  return new urlUtils.UrlBuilder({ path: "/api/realms" });
}

export async function createRealm(payload: CreateOrReplaceRealmPayload): Promise<Realm> {
  const url: string = createUrlBuilder().buildRelative();
  return (await post<CreateOrReplaceRealmPayload, Realm>(url, payload)).data;
}

export async function readRealm(id: string): Promise<Realm> {
  const url: string = createUrlBuilder(id).buildRelative();
  return (await get<Realm>(url)).data;
}

export async function replaceRealm(id: string, payload: CreateOrReplaceRealmPayload, version?: number): Promise<Realm> {
  const url: string = createUrlBuilder(id)
    .setQuery("version", version?.toString() ?? "")
    .buildRelative();
  return (await put<CreateOrReplaceRealmPayload, Realm>(url, payload)).data;
}

export async function searchRealms(payload: SearchRealmsPayload): Promise<SearchResults<Realm>> {
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
  return (await get<SearchResults<Realm>>(url)).data;
}

export async function updateRealm(id: string, payload: UpdateRealmPayload): Promise<Realm> {
  const url: string = createUrlBuilder(id).buildRelative();
  return (await patch<UpdateRealmPayload, Realm>(url, payload)).data;
}
