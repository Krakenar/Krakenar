import type { Locale } from "@/types/i18n";
import type { Role } from "@/types/roles";
import type { User } from "@/types/users";

export function formatLocale(locale: Locale): string {
  return `${locale.displayName} (${locale.code})`;
}

export function formatRole(role: Role): string {
  return role.displayName ? `${role.displayName} (${role.uniqueName})` : role.uniqueName;
}

export function formatUser(user: User): string {
  return user.fullName ? `${user.fullName} (${user.uniqueName})` : user.uniqueName;
}
