import { urlUtils } from "logitar-js";

import type { Message, SearchMessagesPayload, SendMessagePayload, SentMessages } from "@/types/messages";
import { get, post } from ".";
import type { SearchResults } from "@/types/search";

function createUrlBuilder(id?: string): urlUtils.IUrlBuilder {
  if (id) {
    return new urlUtils.UrlBuilder({ path: "/api/messages/{id}" }).setParameter("id", id);
  }
  return new urlUtils.UrlBuilder({ path: "/api/messages" });
}

export async function readMessage(id: string): Promise<Message> {
  const url: string = createUrlBuilder(id).buildRelative();
  return (await get<Message>(url)).data;
}

export async function searchMessages(payload: SearchMessagesPayload): Promise<SearchResults<Message>> {
  const url: string = createUrlBuilder()
    .setQuery("demo", payload.isDemo?.toString() ?? "")
    .setQuery("ids", payload.ids)
    .setQuery(
      "search",
      payload.search.terms.map(({ value }) => value),
    )
    .setQuery("search_operator", payload.search.operator)
    .setQuery("status", payload.status ?? "")
    .setQuery("template", payload.templateId ?? "")
    .setQuery(
      "sort",
      payload.sort.map(({ field, isDescending }) => (isDescending ? `DESC.${field}` : field)),
    )
    .setQuery("skip", payload.skip.toString())
    .setQuery("limit", payload.limit.toString())
    .buildRelative();
  return (await get<SearchResults<Message>>(url)).data;
}

export async function sendMessage(payload: SendMessagePayload): Promise<SentMessages> {
  const url: string = createUrlBuilder().buildRelative();
  return (await post<SendMessagePayload, SentMessages>(url, payload)).data;
}
