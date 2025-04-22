import type { CreateOrReplaceRealmPayload, Realm, SearchRealmsPayload, UpdateRealmPayload } from "@/types/realms";
import type { SearchResults } from "@/types/search";
import { get, patch, post, put } from ".";

export async function createRealm(payload: CreateOrReplaceRealmPayload): Promise<Realm> {
  return (await post<CreateOrReplaceRealmPayload, Realm>("/api/realms", payload)).data;
}

export async function readRealm(id: string): Promise<Realm> {
  return (await get<Realm>(`/api/realms/${id}`)).data;
}

export async function replaceRealm(id: string, payload: CreateOrReplaceRealmPayload, version?: number): Promise<Realm> {
  return (await put<CreateOrReplaceRealmPayload, Realm>(`/api/realms/${id}?version=${version}`, payload)).data;
}

export async function searchRealms(payload: SearchRealmsPayload): Promise<SearchResults<Realm>> {
  return (await get<SearchResults<Realm>>("/api/realms")).data; // TODO(fpion): query string
}

export async function updateRealm(id: string, payload: UpdateRealmPayload): Promise<Realm> {
  return (await patch<UpdateRealmPayload, Realm>(`/api/realms/${id}`, payload)).data;
}
