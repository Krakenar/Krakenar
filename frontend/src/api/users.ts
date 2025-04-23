import { urlUtils } from "logitar-js";

import type { SearchResults } from "@/types/search";
import type { SearchUsersPayload, User } from "@/types/users";
import { get, patch } from ".";

function createUrlBuilder(id?: string): urlUtils.IUrlBuilder {
  if (id) {
    return new urlUtils.UrlBuilder({ path: "/api/users/{id}" }).setParameter("id", id);
  }
  return new urlUtils.UrlBuilder({ path: "/api/users" });
}

export async function searchUsers(payload: SearchUsersPayload): Promise<SearchResults<User>> {
  const url: string = createUrlBuilder()
    .setQuery("authenticated", payload.hasAuthenticated?.toString() ?? "")
    .setQuery("confirmed", payload.isConfirmed?.toString() ?? "")
    .setQuery("disabled", payload.isDisabled?.toString() ?? "")
    .setQuery("ids", payload.ids)
    .setQuery("password", payload.hasPassword?.toString() ?? "")
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
  return (await get<SearchResults<User>>(url)).data;
}

export async function signOutUser(id: string): Promise<User> {
  const url: string = new urlUtils.UrlBuilder({ path: "/api/users/{id}/sign/out" }).setParameter("id", id).buildRelative();
  return (await patch<void, User>(url)).data;
}
