import { urlUtils } from "logitar-js";

import type { FieldType, CreateOrReplaceFieldTypePayload, SearchFieldTypesPayload, UpdateFieldTypePayload } from "@/types/fields";
import type { SearchResults } from "@/types/search";
import { _delete, get, patch, post } from ".";

function createUrlBuilder(id?: string): urlUtils.IUrlBuilder {
  if (id) {
    return new urlUtils.UrlBuilder({ path: "/api/fields/types/{id}" }).setParameter("id", id);
  }
  return new urlUtils.UrlBuilder({ path: "/api/fields/types" });
}

export async function createFieldType(payload: CreateOrReplaceFieldTypePayload): Promise<FieldType> {
  const url: string = createUrlBuilder().buildRelative();
  return (await post<CreateOrReplaceFieldTypePayload, FieldType>(url, payload)).data;
}

export async function deleteFieldType(id: string): Promise<FieldType> {
  const url: string = createUrlBuilder(id).buildRelative();
  return (await _delete<FieldType>(url)).data;
}

export async function readFieldType(id: string): Promise<FieldType> {
  const url: string = createUrlBuilder(id).buildRelative();
  return (await get<FieldType>(url)).data;
}

export async function searchFieldTypes(payload: SearchFieldTypesPayload): Promise<SearchResults<FieldType>> {
  const url: string = createUrlBuilder()
    .setQuery("ids", payload.ids)
    .setQuery(
      "search",
      payload.search.terms.map(({ value }) => value),
    )
    .setQuery("search_operator", payload.search.operator)
    .setQuery("type", payload.dataType ?? "")
    .setQuery(
      "sort",
      payload.sort.map(({ field, isDescending }) => (isDescending ? `DESC.${field}` : field)),
    )
    .setQuery("skip", payload.skip.toString())
    .setQuery("limit", payload.limit.toString())
    .buildRelative();
  return (await get<SearchResults<FieldType>>(url)).data;
}

export async function updateFieldType(id: string, payload: UpdateFieldTypePayload): Promise<FieldType> {
  const url: string = createUrlBuilder(id).buildRelative();
  return (await patch<UpdateFieldTypePayload, FieldType>(url, payload)).data;
}
