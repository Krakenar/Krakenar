import { urlUtils } from "logitar-js";

import type { CreateOrReplaceFieldDefinitionPayload } from "@/types/fields";
import { _delete, post, put } from "..";
import type { ContentType } from "@/types/contents";

function createUrlBuilder(contentTypeId: string, fieldId?: string): urlUtils.IUrlBuilder {
  if (fieldId) {
    return new urlUtils.UrlBuilder({ path: "/api/contents/types/{contentTypeId}/fields/{fieldId}" })
      .setParameter("contentTypeId", contentTypeId)
      .setParameter("fieldId", fieldId);
  }
  return new urlUtils.UrlBuilder({ path: "/api/contents/types/{contentTypeId}/fields" }).setParameter("contentTypeId", contentTypeId);
}

export async function createFieldDefinition(contentTypeId: string, payload: CreateOrReplaceFieldDefinitionPayload): Promise<ContentType> {
  const url: string = createUrlBuilder(contentTypeId).buildRelative();
  return (await post<CreateOrReplaceFieldDefinitionPayload, ContentType>(url, payload)).data;
}

export async function deleteFieldDefinition(contentTypeId: string, fieldId: string): Promise<ContentType> {
  const url: string = createUrlBuilder(contentTypeId, fieldId).buildRelative();
  return (await _delete<ContentType>(url)).data;
}

export async function replaceFieldDefinition(contentTypeId: string, fieldId: string, payload: CreateOrReplaceFieldDefinitionPayload): Promise<ContentType> {
  const url: string = createUrlBuilder(contentTypeId, fieldId).buildRelative();
  return (await put<CreateOrReplaceFieldDefinitionPayload, ContentType>(url, payload)).data;
}
