import type { ContentLocale, ContentType } from "@/types/contents";
import type { FieldDefinition, FieldType } from "@/types/fields";
import type { Locale } from "@/types/i18n";
import type { Realm } from "@/types/realms";
import type { Role } from "@/types/roles";
import type { Sender } from "@/types/senders";
import type { Template } from "@/types/templates";
import type { User } from "@/types/users";

export function formatContentLocale(locale: ContentLocale): string {
  return locale.displayName ? `${locale.displayName} (${locale.uniqueName})` : locale.uniqueName;
}

export function formatContentType(contentType: ContentType): string {
  return contentType.displayName ? `${contentType.displayName} (${contentType.uniqueName})` : contentType.uniqueName;
}

export function formatFieldDefinition(fieldDefinition: FieldDefinition): string {
  return fieldDefinition.displayName ? `${fieldDefinition.displayName} (${fieldDefinition.uniqueName})` : fieldDefinition.uniqueName;
}

export function formatFieldType(fieldType: FieldType): string {
  return fieldType.displayName ? `${fieldType.displayName} (${fieldType.uniqueName})` : fieldType.uniqueName;
}

export function formatLocale(locale: Locale): string {
  return `${locale.displayName} (${locale.code})`;
}

export function formatRealm(realm: Realm): string {
  return realm.displayName ? `${realm.displayName} (${realm.uniqueSlug})` : realm.uniqueSlug;
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
