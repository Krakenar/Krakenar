import type { Aggregate } from "./aggregate";
import type { CustomAttribute } from "./custom";
import type { Realm } from "./realms";

export type Role = Aggregate & {
  realm?: Realm;
  uniqueName: string;
  displayName?: string;
  description?: string;
  customAttributes: CustomAttribute[];
};
