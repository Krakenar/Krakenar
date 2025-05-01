import { urlUtils } from "logitar-js";

import type { SearchResults } from "@/types/search";
import type { CreateOrReplaceUserPayload, SearchUsersPayload, UpdateUserPayload, User } from "@/types/users";
import { _delete, get, patch, post, put } from ".";

function createUrlBuilder(id?: string): urlUtils.IUrlBuilder {
  if (id) {
    return new urlUtils.UrlBuilder({ path: "/api/users/{id}" }).setParameter("id", id);
  }
  return new urlUtils.UrlBuilder({ path: "/api/users" });
}

export async function createUser(payload: CreateOrReplaceUserPayload): Promise<User> {
  const url: string = createUrlBuilder().buildRelative();
  return (await post<CreateOrReplaceUserPayload, User>(url, payload)).data;
}

export async function deleteUser(id: string): Promise<User> {
  const url: string = createUrlBuilder(id).buildRelative();
  return (await _delete<User>(url)).data;
}

export async function readUser(id: string): Promise<User> {
  const url: string = createUrlBuilder(id).buildRelative();
  return (await get<User>(url)).data;
}

export async function replaceUser(id: string, payload: CreateOrReplaceUserPayload, version?: number): Promise<User> {
  const url: string = createUrlBuilder(id)
    .setQuery("version", version?.toString() ?? "")
    .buildRelative();
  return (await put<CreateOrReplaceUserPayload, User>(url, payload)).data;
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

export async function updateUser(id: string, payload: UpdateUserPayload): Promise<User> {
  const url: string = createUrlBuilder(id).buildRelative();
  return (await patch<UpdateUserPayload, User>(url, payload)).data;
}
