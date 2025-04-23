import { urlUtils } from "logitar-js";

import type { CurrentUser, SignInAccountPayload } from "@/types/account";
import { post } from "./index";

export async function signIn(payload: SignInAccountPayload): Promise<CurrentUser> {
  const url: string = new urlUtils.UrlBuilder({ path: "/api/sign/in" }).buildRelative();
  return (await post<SignInAccountPayload, CurrentUser>(url, payload)).data;
}
