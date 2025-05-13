import { urlUtils } from "logitar-js";

import type { CreateOrReplaceSenderPayload, Sender, SearchSendersPayload, UpdateSenderPayload } from "@/types/senders";
import type { SearchResults } from "@/types/search";
import { _delete, get, patch, post, put } from ".";

function createUrlBuilder(id?: string): urlUtils.IUrlBuilder {
  if (id) {
    return new urlUtils.UrlBuilder({ path: "/api/senders/{id}" }).setParameter("id", id);
  }
  return new urlUtils.UrlBuilder({ path: "/api/senders" });
}

export async function createSender(payload: CreateOrReplaceSenderPayload): Promise<Sender> {
  const url: string = createUrlBuilder().buildRelative();
  return (await post<CreateOrReplaceSenderPayload, Sender>(url, payload)).data;
}

export async function deleteSender(id: string): Promise<Sender> {
  const url: string = createUrlBuilder(id).buildRelative();
  return (await _delete<Sender>(url)).data;
}

export async function readSender(id: string): Promise<Sender> {
  const url: string = createUrlBuilder(id).buildRelative();
  return (await get<Sender>(url)).data;
}

export async function replaceSender(id: string, payload: CreateOrReplaceSenderPayload, version?: number): Promise<Sender> {
  const url: string = createUrlBuilder(id)
    .setQuery("version", version?.toString() ?? "")
    .buildRelative();
  return (await put<CreateOrReplaceSenderPayload, Sender>(url, payload)).data;
}

export async function searchSenders(payload: SearchSendersPayload): Promise<SearchResults<Sender>> {
  const url: string = createUrlBuilder()
    .setQuery("ids", payload.ids)
    .setQuery("kind", payload.kind ?? "")
    .setQuery("provider", payload.provider ?? "")
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
  return (await get<SearchResults<Sender>>(url)).data;
}

export async function setDefaultSender(id: string): Promise<Sender> {
  const url: string = new urlUtils.UrlBuilder({ path: "/api/senders/{id}/default" }).setParameter("id", id).buildRelative();
  return (await patch<void, Sender>(url)).data;
}

export async function updateSender(id: string, payload: UpdateSenderPayload): Promise<Sender> {
  const url: string = createUrlBuilder(id).buildRelative();
  return (await patch<UpdateSenderPayload, Sender>(url, payload)).data;
}
