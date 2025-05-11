import { urlUtils } from "logitar-js";

import type { CreateOrReplaceTemplatePayload, Template, SearchTemplatesPayload, UpdateTemplatePayload } from "@/types/templates";
import type { SearchResults } from "@/types/search";
import { _delete, get, patch, post, put } from ".";

function createUrlBuilder(id?: string): urlUtils.IUrlBuilder {
  if (id) {
    return new urlUtils.UrlBuilder({ path: "/api/templates/{id}" }).setParameter("id", id);
  }
  return new urlUtils.UrlBuilder({ path: "/api/templates" });
}

export async function createTemplate(payload: CreateOrReplaceTemplatePayload): Promise<Template> {
  const url: string = createUrlBuilder().buildRelative();
  return (await post<CreateOrReplaceTemplatePayload, Template>(url, payload)).data;
}

export async function deleteTemplate(id: string): Promise<Template> {
  const url: string = createUrlBuilder(id).buildRelative();
  return (await _delete<Template>(url)).data;
}

export async function readTemplate(id: string): Promise<Template> {
  const url: string = createUrlBuilder(id).buildRelative();
  return (await get<Template>(url)).data;
}

export async function replaceTemplate(id: string, payload: CreateOrReplaceTemplatePayload, version?: number): Promise<Template> {
  const url: string = createUrlBuilder(id)
    .setQuery("version", version?.toString() ?? "")
    .buildRelative();
  return (await put<CreateOrReplaceTemplatePayload, Template>(url, payload)).data;
}

export async function searchTemplates(payload: SearchTemplatesPayload): Promise<SearchResults<Template>> {
  const url: string = createUrlBuilder()
    .setQuery("ids", payload.ids)
    .setQuery(
      "search",
      payload.search.terms.map(({ value }) => value),
    )
    .setQuery("search_operator", payload.search.operator)
    .setQuery("type", payload.contentType ?? "")
    .setQuery(
      "sort",
      payload.sort.map(({ field, isDescending }) => (isDescending ? `DESC.${field}` : field)),
    )
    .setQuery("skip", payload.skip.toString())
    .setQuery("limit", payload.limit.toString())
    .buildRelative();
  return (await get<SearchResults<Template>>(url)).data;
}

export async function updateTemplate(id: string, payload: UpdateTemplatePayload): Promise<Template> {
  const url: string = createUrlBuilder(id).buildRelative();
  return (await patch<UpdateTemplatePayload, Template>(url, payload)).data;
}
