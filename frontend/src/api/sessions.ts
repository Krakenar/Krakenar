import type { Session } from "@/types/sessions";
import { patch } from ".";

export async function signOut(id: string): Promise<Session> {
  return (await patch<void, Session>(`/api/sessions/${id}/sign/out`)).data;
}
