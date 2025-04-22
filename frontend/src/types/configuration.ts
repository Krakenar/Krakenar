import type { Aggregate } from "./aggregate";
import type { LoggingSettings, PasswordSettings, UniqueNameSettings } from "./settings";

export type Configuration = Aggregate & {
  uniqueNameSettings: UniqueNameSettings;
  passwordSettings: PasswordSettings;
  loggingSettings: LoggingSettings;
};

export type ReplaceConfigurationPayload = {
  uniqueNameSettings: UniqueNameSettings;
  passwordSettings: PasswordSettings;
  loggingSettings: LoggingSettings;
};
