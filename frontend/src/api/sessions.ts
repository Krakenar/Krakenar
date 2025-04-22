import { urlUtils } from "logitar-js";

import type { Session } from "@/types/sessions";
import { patch } from ".";

function createUrlBuilder(id?: string): urlUtils.IUrlBuilder {
  if (id) {
    return new urlUtils.UrlBuilder({ path: "/api/sessions/{id}" }).setParameter("id", id);
  }
  return new urlUtils.UrlBuilder({ path: "/api/sessions" });
}

export async function signOut(id: string): Promise<Session> {
  const url: string = new urlUtils.UrlBuilder({ path: "/api/sessions/{id}/sign/out" }).setParameter("id", id).buildRelative();
  return (await patch<void, Session>(url)).data;
}
