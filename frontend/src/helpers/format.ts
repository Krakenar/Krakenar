import type { Locale } from "@/types/i18n";
import type { Role } from "@/types/roles";
import type { Sender } from "@/types/senders";
import type { Template } from "@/types/templates";
import type { User } from "@/types/users";

export function formatLocale(locale: Locale): string {
  return `${locale.displayName} (${locale.code})`;
}

export function formatRole(role: Role): string {
  return role.displayName ? `${role.displayName} (${role.uniqueName})` : role.uniqueName;
}

export function formatSender(sender: Sender): string {
  let contact: string = "";
  switch (sender.kind) {
    case "Email":
      contact = sender.email?.address ?? contact;
      break;
    case "Phone":
      contact = sender.phone?.e164Formatted ?? contact;
      break;
  }
  return sender.displayName ? `${sender.displayName} <${contact}>` : contact;
}

export function formatTemplate(template: Template): string {
  return template.displayName ? `${template.displayName} (${template.uniqueName})` : template.uniqueName;
}

export function formatUser(user: User): string {
  return user.fullName ? `${user.fullName} (${user.uniqueName})` : user.uniqueName;
}
