import { urlUtils } from "logitar-js";

import type { CreateOrReplaceRolePayload, Role, SearchRolesPayload, UpdateRolePayload } from "@/types/roles";
import type { SearchResults } from "@/types/search";
import { _delete, get, patch, post, put } from ".";

function createUrlBuilder(id?: string): urlUtils.IUrlBuilder {
  if (id) {
    return new urlUtils.UrlBuilder({ path: `/api/roles/${id}` }).setParameter("id", id);
  }
  return new urlUtils.UrlBuilder({ path: `/api/roles` });
}

export async function createRole(payload: CreateOrReplaceRolePayload): Promise<Role> {
  const url: string = createUrlBuilder().buildRelative();
  return (await post<CreateOrReplaceRolePayload, Role>(url, payload)).data;
}

export async function deleteRole(id: string): Promise<Role> {
  const url: string = createUrlBuilder(id).buildRelative();
  return (await _delete<Role>(url)).data;
}

export async function readRole(id: string): Promise<Role> {
  const url: string = createUrlBuilder(id).buildRelative();
  return (await get<Role>(url)).data;
}

export async function replaceRole(id: string, payload: CreateOrReplaceRolePayload, version?: number): Promise<Role> {
  const url: string = createUrlBuilder(id)
    .setQuery("version", version?.toString() ?? "")
    .buildRelative();
  return (await put<CreateOrReplaceRolePayload, Role>(url, payload)).data;
}

export async function searchRoles(payload: SearchRolesPayload): Promise<SearchResults<Role>> {
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
  return (await get<SearchResults<Role>>(url)).data;
}

export async function updateRole(id: string, payload: UpdateRolePayload): Promise<Role> {
  const url: string = createUrlBuilder(id).buildRelative();
  return (await patch<UpdateRolePayload, Role>(url, payload)).data;
}
