import { urlUtils } from "logitar-js";

import type { CreatedToken, CreateTokenPayload, ValidatedToken, ValidateTokenPayload } from "@/types/tokens";
import { post, put } from ".";

export async function createToken(payload: CreateTokenPayload): Promise<CreatedToken> {
  const url: string = new urlUtils.UrlBuilder({ path: "/api/tokens" }).buildRelative();
  return (await post<CreateTokenPayload, CreatedToken>(url, payload)).data;
}

export async function validateToken(payload: ValidateTokenPayload): Promise<ValidatedToken> {
  const url: string = new urlUtils.UrlBuilder({ path: "/api/tokens" }).buildRelative();
  return (await put<ValidateTokenPayload, ValidatedToken>(url, payload)).data;
}
