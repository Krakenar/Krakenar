import type { ApiKey } from "@/types/apiKeys";

export function isExpired(apiKey: ApiKey, moment?: Date): boolean {
  return typeof apiKey.expiresOn === "string" && new Date(apiKey.expiresOn) <= (moment ?? new Date());
}
