import type { CurrentUser, SignInAccountPayload } from "@/types/account";
import { post } from "./index";

export async function signIn(payload: SignInAccountPayload): Promise<CurrentUser> {
  return (await post<SignInAccountPayload, CurrentUser>("/api/sign/in", payload)).data;
}
