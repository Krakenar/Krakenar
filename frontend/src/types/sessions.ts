import type { Actor } from "./actor";
import type { Aggregate } from "./aggregate";
import type { CustomAttribute } from "./custom";
import type { User } from "./users";

export type Session = Aggregate & {
  user: User;
  isPersistent: boolean;
  refreshToken?: string | null;
  isActive: boolean;
  signedOutBy?: Actor | null;
  signedOutOn?: string | null;
  customAttributes: CustomAttribute[];
};
