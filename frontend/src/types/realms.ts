import type { Aggregate } from "./aggregate";
import type { CustomAttribute } from "./custom";
import type { PasswordSettings, UniqueNameSettings } from "./settings";

export type Realm = Aggregate & {
  uniqueSlug: string;
  displayName?: string | null;
  description?: string | null;
  url?: string | null;
  uniqueNameSettings: UniqueNameSettings;
  passwordSettings: PasswordSettings;
  requireUniqueEmail: boolean;
  requireConfirmedAccount: boolean;
  customAttributes: CustomAttribute[];
};
