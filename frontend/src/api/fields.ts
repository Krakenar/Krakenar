import { urlUtils } from "logitar-js";

import type {
  FieldType,
  CreateOrReplaceFieldTypePayload,
  SearchFieldTypesPayload,
  UpdateFieldTypePayload,
  CreateOrReplaceFieldDefinitionPayload,
} from "@/types/fields";
import type { SearchResults } from "@/types/search";
import { _delete, get, patch, post, put } from ".";
import type { ContentType } from "@/types/contents";

function createDefinitionUrlBuilder(contentTypeId: string, fieldId?: string): urlUtils.IUrlBuilder {
  if (fieldId) {
    return new urlUtils.UrlBuilder({ path: "/api/contents/types/{contentTypeId}/fields/{fieldId}" })
      .setParameter("contentTypeId", contentTypeId)
      .setParameter("fieldId", fieldId);
  }
  return new urlUtils.UrlBuilder({ path: "/api/contents/types/{contentTypeId}/fields" }).setParameter("contentTypeId", contentTypeId);
}

function createTypeUrlBuilder(id?: string): urlUtils.IUrlBuilder {
  if (id) {
    return new urlUtils.UrlBuilder({ path: "/api/fields/types/{id}" }).setParameter("id", id);
  }
  return new urlUtils.UrlBuilder({ path: "/api/fields/types" });
}

export async function createFieldDefinition(contentTypeId: string, payload: CreateOrReplaceFieldDefinitionPayload): Promise<ContentType> {
  const url: string = createDefinitionUrlBuilder(contentTypeId).buildRelative();
  return (await post<CreateOrReplaceFieldDefinitionPayload, ContentType>(url, payload)).data;
}

export async function createFieldType(payload: CreateOrReplaceFieldTypePayload): Promise<FieldType> {
  const url: string = createTypeUrlBuilder().buildRelative();
  return (await post<CreateOrReplaceFieldTypePayload, FieldType>(url, payload)).data;
}

export async function deleteFieldDefinition(contentTypeId: string, fieldId: string): Promise<ContentType> {
  const url: string = createDefinitionUrlBuilder(contentTypeId, fieldId).buildRelative();
  return (await _delete<ContentType>(url)).data;
}

export async function deleteFieldType(id: string): Promise<FieldType> {
  const url: string = createTypeUrlBuilder(id).buildRelative();
  return (await _delete<FieldType>(url)).data;
}

export async function readFieldType(id: string): Promise<FieldType> {
  const url: string = createTypeUrlBuilder(id).buildRelative();
  return (await get<FieldType>(url)).data;
}

export async function replaceFieldDefinition(contentTypeId: string, fieldId: string, payload: CreateOrReplaceFieldDefinitionPayload): Promise<ContentType> {
  const url: string = createDefinitionUrlBuilder(contentTypeId, fieldId).buildRelative();
  return (await put<CreateOrReplaceFieldDefinitionPayload, ContentType>(url, payload)).data;
}

export async function searchFieldTypes(payload: SearchFieldTypesPayload): Promise<SearchResults<FieldType>> {
  const url: string = createTypeUrlBuilder()
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
  const url: string = createTypeUrlBuilder(id).buildRelative();
  return (await patch<UpdateFieldTypePayload, FieldType>(url, payload)).data;
}
