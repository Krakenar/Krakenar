import { urlUtils } from "logitar-js";

import type { Configuration, ReplaceConfigurationPayload, UpdateConfigurationPayload } from "@/types/configuration";
import { get, patch, put } from ".";

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

export async function updateConfiguration(payload: UpdateConfigurationPayload): Promise<Configuration> {
  const url: string = createUrlBuilder().buildRelative();
  return (await patch<UpdateConfigurationPayload, Configuration>(url, payload)).data;
}
