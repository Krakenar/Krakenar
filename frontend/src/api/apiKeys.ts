import { urlUtils } from "logitar-js";

import type { ApiKey, CreateOrReplaceApiKeyPayload, SearchApiKeysPayload, UpdateApiKeyPayload } from "@/types/apiKeys";
import type { SearchResults } from "@/types/search";
import { _delete, get, patch, post } from ".";

function createUrlBuilder(id?: string): urlUtils.IUrlBuilder {
  if (id) {
    return new urlUtils.UrlBuilder({ path: "/api/keys/{id}" }).setParameter("id", id);
  }
  return new urlUtils.UrlBuilder({ path: "/api/keys" });
}

export async function createApiKey(payload: CreateOrReplaceApiKeyPayload): Promise<ApiKey> {
  const url: string = createUrlBuilder().buildRelative();
  return (await post<CreateOrReplaceApiKeyPayload, ApiKey>(url, payload)).data;
}

export async function deleteApiKey(id: string): Promise<ApiKey> {
  const url: string = createUrlBuilder(id).buildRelative();
  return (await _delete<ApiKey>(url)).data;
}

export async function readApiKey(id: string): Promise<ApiKey> {
  const url: string = createUrlBuilder(id).buildRelative();
  return (await get<ApiKey>(url)).data;
}

export async function searchApiKeys(payload: SearchApiKeysPayload): Promise<SearchResults<ApiKey>> {
  const url: string = createUrlBuilder()
    .setQuery("authenticated", payload.hasAuthenticated?.toString() ?? "")
    .setQuery("expired", payload.status?.isExpired.toString() ?? "")
    .setQuery("ids", payload.ids)
    .setQuery("moment", payload.status?.moment?.toISOString() ?? "")
    .setQuery("role", payload.roleId ?? "")
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
  return (await get<SearchResults<ApiKey>>(url)).data;
}

export async function updateApiKey(id: string, payload: UpdateApiKeyPayload): Promise<ApiKey> {
  const url: string = createUrlBuilder(id).buildRelative();
  return (await patch<UpdateApiKeyPayload, ApiKey>(url, payload)).data;
}
