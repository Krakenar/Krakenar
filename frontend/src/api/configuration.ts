import type { Configuration, ReplaceConfigurationPayload } from "@/types/configuration";
import { get, put } from ".";

export async function readConfiguration(): Promise<Configuration> {
  return (await get<Configuration>("/api/configuration")).data;
}

export async function replaceConfiguration(payload: ReplaceConfigurationPayload, version?: number): Promise<Configuration> {
  return (await put<ReplaceConfigurationPayload, Configuration>(`/api/configuration?version=${version}`, payload)).data;
}
