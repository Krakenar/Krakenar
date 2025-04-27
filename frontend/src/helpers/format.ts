import type { Locale } from "@/types/i18n";
import type { User } from "@/types/users";

export function formatLocale(locale: Locale): string {
  return `${locale.displayName} (${locale.code})`;
}

export function formatUser(user: User): string {
  return user.fullName ? `${user.fullName} (${user.uniqueName})` : user.uniqueName;
}
