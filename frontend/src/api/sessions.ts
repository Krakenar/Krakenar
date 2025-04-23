import { urlUtils } from "logitar-js";

import type { SearchResults } from "@/types/search";
import type { SearchSessionsPayload, Session } from "@/types/sessions";
import { get, patch } from ".";

function createUrlBuilder(id?: string): urlUtils.IUrlBuilder {
  if (id) {
    return new urlUtils.UrlBuilder({ path: "/api/sessions/{id}" }).setParameter("id", id);
  }
  return new urlUtils.UrlBuilder({ path: "/api/sessions" });
}

export async function searchSessions(payload: SearchSessionsPayload): Promise<SearchResults<Session>> {
  const url: string = createUrlBuilder()
    .setQuery("active", payload.isActive?.toString() ?? "")
    .setQuery("ids", payload.ids)
    .setQuery("persistent", payload.isPersistent?.toString() ?? "")
    .setQuery(
      "search",
      payload.search.terms.map(({ value }) => value),
    )
    .setQuery("search_operator", payload.search.operator)
    .setQuery("user", payload.userId ?? "")
    .setQuery(
      "sort",
      payload.sort.map(({ field, isDescending }) => (isDescending ? `DESC.${field}` : field)),
    )
    .setQuery("skip", payload.skip.toString())
    .setQuery("limit", payload.limit.toString())
    .buildRelative();
  return (await get<SearchResults<Session>>(url)).data;
}

export async function signOutSession(id: string): Promise<Session> {
  const url: string = new urlUtils.UrlBuilder({ path: "/api/sessions/{id}/sign/out" }).setParameter("id", id).buildRelative();
  return (await patch<void, Session>(url)).data;
}
