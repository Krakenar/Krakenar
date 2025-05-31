import { urlUtils } from "logitar-js";

import type { Statistics } from "@/types/dashboard";
import { get } from ".";

export async function getStatistics(): Promise<Statistics> {
  const url: string = new urlUtils.UrlBuilder({ path: "/api/dashboard" }).buildRelative();
  return (await get<Statistics>(url)).data;
}
