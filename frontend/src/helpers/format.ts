import type { Locale } from "@/types/i18n";
import type { Role } from "@/types/roles";
import type { Template } from "@/types/templates";
import type { User } from "@/types/users";

export function formatLocale(locale: Locale): string {
  return `${locale.displayName} (${locale.code})`;
}

export function formatRole(role: Role): string {
  return role.displayName ? `${role.displayName} (${role.uniqueName})` : role.uniqueName;
}

export function formatTemplate(template: Template): string {
  return template.displayName ? `${template.displayName} (${template.uniqueName})` : template.uniqueName;
}

export function formatUser(user: User): string {
  return user.fullName ? `${user.fullName} (${user.uniqueName})` : user.uniqueName;
}
