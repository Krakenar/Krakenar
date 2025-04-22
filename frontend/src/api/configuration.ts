import { urlUtils } from "logitar-js";

import type { Configuration, ReplaceConfigurationPayload } from "@/types/configuration";
import { get, put } from ".";

function createUrlBuilder(): urlUtils.IUrlBuilder {
  return new urlUtils.UrlBuilder({ path: "/api/configuration" });
}

export async function readConfiguration(): Promise<Configuration> {
  const url: string = createUrlBuilder().buildRelative();
  return (await get<Configuration>(url)).data;
}

export async function replaceConfiguration(payload: ReplaceConfigurationPayload, version?: number): Promise<Configuration> {
  const url: string = createUrlBuilder()
    .setQuery("version", version?.toString() ?? "")
    .buildRelative();
  return (await put<ReplaceConfigurationPayload, Configuration>(url, payload)).data;
}
