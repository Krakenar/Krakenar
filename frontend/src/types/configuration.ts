import type { Actor } from "./actor";
import type { Aggregate } from "./aggregate";
import type { Change } from "./change";
import type { LoggingSettings, PasswordSettings, UniqueNameSettings } from "./settings";

export type Configuration = Aggregate & {
  secretChangedBy: Actor;
  secretChangedOn: string;
  uniqueNameSettings: UniqueNameSettings;
  passwordSettings: PasswordSettings;
  loggingSettings: LoggingSettings;
};

export type ReplaceConfigurationPayload = {
  uniqueNameSettings: UniqueNameSettings;
  passwordSettings: PasswordSettings;
  loggingSettings: LoggingSettings;
};

export type UpdateConfigurationPayload = {
  secret?: Change<string>;
  uniqueNameSettings?: UniqueNameSettings;
  passwordSettings?: PasswordSettings;
  loggingSettings?: LoggingSettings;
};
