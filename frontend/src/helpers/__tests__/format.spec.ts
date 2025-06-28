import { describe, it, expect } from "vitest";
import { nanoid } from "nanoid";

import type { Actor } from "@/types/actor";
import type { ContentLocale, ContentType } from "@/types/contents";
import type { FieldDefinition, FieldType } from "@/types/fields";
import type { Locale } from "@/types/i18n";
import type { Realm } from "@/types/realms";
import type { Role } from "@/types/roles";
import type { Sender } from "@/types/senders";
import type { Template } from "@/types/templates";
import type { User } from "@/types/users";
import {
  formatContentLocale,
  formatContentType,
  formatFieldDefinition,
  formatFieldType,
  formatLocale,
  formatRealm,
  formatRole,
  formatSender,
  formatTemplate,
  formatUser,
} from "../format";

const actor: Actor = {
  type: "User",
  id: nanoid(),
  isDeleted: false,
  displayName: "Administrator",
};
const now: string = new Date().toISOString();

const contentType: ContentType = {
  id: nanoid(),
  version: 0,
  createdBy: actor,
  createdOn: now,
  updatedBy: actor,
  updatedOn: now,
  isInvariant: false,
  uniqueName: "BlogArticle",
  fieldCount: 0,
  fields: [],
};
const contentLocale: ContentLocale = {
  uniqueName: "acura-tlx",
  fieldValues: [],
  version: 0,
  createdBy: actor,
  createdOn: now,
  updatedBy: actor,
  updatedOn: now,
  isPublished: false,
};
const fieldType: FieldType = {
  id: nanoid(),
  version: 0,
  createdBy: actor,
  createdOn: now,
  updatedBy: actor,
  updatedOn: now,
  uniqueName: "ArticleTitle",
  dataType: "RichText",
};
const fieldDefinition: FieldDefinition = {
  id: nanoid(),
  order: 0,
  fieldType,
  isInvariant: false,
  isRequired: false,
  isIndexed: false,
  isUnique: false,
  uniqueName: "Keywords",
  createdBy: actor,
  createdOn: now,
  updatedBy: actor,
  updatedOn: now,
};

describe("formatContentLocale", () => {
  it.concurrent("should format the content locale with display name correctly", () => {
    const subject: ContentLocale = { ...contentLocale, displayName: "Acura TLX" };
    expect(formatContentLocale(subject)).toBe("Acura TLX (acura-tlx)");
  });

  it.concurrent("should format the content locale without display name correctly", () => {
    expect(formatContentLocale(contentLocale)).toBe("acura-tlx");
  });
});

describe("formatContentType", () => {
  it.concurrent("should format the content type with display name correctly", () => {
    const subject: ContentType = { ...contentType, displayName: "Blog Article" };
    expect(formatContentType(subject)).toBe("Blog Article (BlogArticle)");
  });

  it.concurrent("should format the content type without display name correctly", () => {
    expect(formatContentType(contentType)).toBe("BlogArticle");
  });
});

describe("formatFieldDefinition", () => {
  it.concurrent("should format the field definition with display name correctly", () => {
    const subject: FieldDefinition = { ...fieldDefinition, displayName: "Meta Keywords" };
    expect(formatFieldDefinition(subject)).toBe("Meta Keywords (Keywords)");
  });

  it.concurrent("should format the field definition without display name correctly", () => {
    expect(formatFieldDefinition(fieldDefinition)).toBe("Keywords");
  });
});

describe("formatFieldType", () => {
  it.concurrent("should format the field type with display name correctly", () => {
    const subject: FieldType = { ...fieldType, displayName: "Article Title" };
    expect(formatFieldType(subject)).toBe("Article Title (ArticleTitle)");
  });

  it.concurrent("should format the field type without display name correctly", () => {
    expect(formatFieldType(fieldType)).toBe("ArticleTitle");
  });
});

describe("formatLocale", () => {
  it.concurrent("should format the locale (with region) correctly", () => {
    const locale: Locale = {
      id: 3084,
      code: "fr-CA",
      displayName: "French (Canada)",
      englishName: "French (Canada)",
      nativeName: "fran\u00E7ais (Canada)",
    };
    expect(formatLocale(locale)).toBe("French (Canada) (fr-CA)");
  });

  it.concurrent("should format the locale (without region) correctly", () => {
    const locale: Locale = {
      id: 9,
      code: "en",
      displayName: "English",
      englishName: "English",
      nativeName: "English",
    };
    expect(formatLocale(locale)).toBe("English (en)");
  });
});

describe("formatRealm", () => {
  it.concurrent("should format the realm with display name correctly", () => {
    const realm: Realm = {
      id: nanoid(),
      version: 0,
      createdBy: actor,
      createdOn: now,
      updatedBy: actor,
      updatedOn: now,
      uniqueSlug: "new-world",
      displayName: "The New World",
      secretChangedBy: actor,
      secretChangedOn: now,
      uniqueNameSettings: {},
      passwordSettings: {
        requiredLength: 8,
        requiredUniqueChars: 8,
        requireDigit: true,
        requireLowercase: true,
        requireUppercase: true,
        requireNonAlphanumeric: true,
        hashingStrategy: "PBKDF2",
      },
      requireUniqueEmail: true,
      requireConfirmedAccount: true,
      customAttributes: [],
    };
    expect(formatRealm(realm)).toBe("The New World (new-world)");
  });

  it.concurrent("should format the realm without display name correctly", () => {
    const realm: Realm = {
      id: nanoid(),
      version: 0,
      createdBy: actor,
      createdOn: now,
      updatedBy: actor,
      updatedOn: now,
      uniqueSlug: "new-world",
      secretChangedBy: actor,
      secretChangedOn: now,
      uniqueNameSettings: {},
      passwordSettings: {
        requiredLength: 8,
        requiredUniqueChars: 8,
        requireDigit: true,
        requireLowercase: true,
        requireUppercase: true,
        requireNonAlphanumeric: true,
        hashingStrategy: "PBKDF2",
      },
      requireUniqueEmail: true,
      requireConfirmedAccount: true,
      customAttributes: [],
    };
    expect(formatRealm(realm)).toBe("new-world");
  });
});

describe("formatRole", () => {
  it.concurrent("should format the role with display name correctly", () => {
    const role: Role = {
      id: nanoid(),
      version: 0,
      createdBy: actor,
      createdOn: now,
      updatedBy: actor,
      updatedOn: now,
      uniqueName: "admin",
      displayName: "Administrator",
      customAttributes: [],
    };
    expect(formatRole(role)).toBe("Administrator (admin)");
  });

  it.concurrent("should format the role without display name correctly", () => {
    const role: Role = {
      id: nanoid(),
      version: 0,
      createdBy: actor,
      createdOn: now,
      updatedBy: actor,
      updatedOn: now,
      uniqueName: "admin",
      customAttributes: [],
    };
    expect(formatRole(role)).toBe("admin");
  });
});

describe("formatSender", () => {
  it.concurrent("should format the email sender with display name correctly", () => {
    const sender: Sender = {
      id: nanoid(),
      version: 0,
      createdBy: actor,
      createdOn: now,
      updatedBy: actor,
      updatedOn: now,
      kind: "Email",
      isDefault: true,
      email: { address: "no-reply@krakenar.com", isVerified: false },
      displayName: "Krakenar",
      provider: "SendGrid",
    };
    expect(formatSender(sender)).toBe("Krakenar <no-reply@krakenar.com>");
  });

  it.concurrent("should format the email sender without display name correctly", () => {
    const sender: Sender = {
      id: nanoid(),
      version: 0,
      createdBy: actor,
      createdOn: now,
      updatedBy: actor,
      updatedOn: now,
      kind: "Email",
      isDefault: true,
      email: { address: "no-reply@krakenar.com", isVerified: false },
      provider: "SendGrid",
    };
    expect(formatSender(sender)).toBe("no-reply@krakenar.com");
  });

  it.concurrent("should format the phone sender with display name correctly", () => {
    const sender: Sender = {
      id: nanoid(),
      version: 0,
      createdBy: actor,
      createdOn: now,
      updatedBy: actor,
      updatedOn: now,
      kind: "Phone",
      isDefault: true,
      phone: { countryCode: "CA", number: "2345678900", e164Formatted: "+12345678900", isVerified: false },
      displayName: "Krakenar",
      provider: "Twilio",
    };
    expect(formatSender(sender)).toBe("Krakenar <+12345678900>");
  });

  it.concurrent("should format the phone sender without display name correctly", () => {
    const sender: Sender = {
      id: nanoid(),
      version: 0,
      createdBy: actor,
      createdOn: now,
      updatedBy: actor,
      updatedOn: now,
      kind: "Phone",
      isDefault: true,
      phone: { countryCode: "CA", number: "2345678900", e164Formatted: "+12345678900", isVerified: false },
      provider: "Twilio",
    };
    expect(formatSender(sender)).toBe("+12345678900");
  });
});

describe("formatTemplate", () => {
  it.concurrent("should format the template with display name correctly", () => {
    const template: Template = {
      id: nanoid(),
      version: 0,
      createdBy: actor,
      createdOn: now,
      updatedBy: actor,
      updatedOn: now,
      uniqueName: "PasswordRecovery",
      displayName: "Password Recovery",
      subject: "PasswordRecovery_Subject",
      content: { type: "text/html", text: "<div>Hello World!</div>" },
    };
    expect(formatTemplate(template)).toBe("Password Recovery (PasswordRecovery)");
  });

  it.concurrent("should format the template without display name correctly", () => {
    const template: Template = {
      id: nanoid(),
      version: 0,
      createdBy: actor,
      createdOn: now,
      updatedBy: actor,
      updatedOn: now,
      uniqueName: "PasswordRecovery",
      subject: "PasswordRecovery_Subject",
      content: { type: "text/plain", text: "Hello World!" },
    };
    expect(formatTemplate(template)).toBe("PasswordRecovery");
  });
});

describe("formatUser", () => {
  it.concurrent("should format the user with full name correctly", () => {
    const user: User = {
      id: actor.id,
      version: 0,
      createdBy: actor,
      createdOn: now,
      updatedBy: actor,
      updatedOn: now,
      uniqueName: "admin",
      hasPassword: false,
      isDisabled: false,
      isConfirmed: false,
      fullName: actor.displayName,
      customAttributes: [],
      customIdentifiers: [],
      roles: [],
      sessions: [],
    };
    expect(formatUser(user)).toBe("Administrator (admin)");
  });

  it.concurrent("should format the user without full name correctly", () => {
    const user: User = {
      id: actor.id,
      version: 0,
      createdBy: actor,
      createdOn: now,
      updatedBy: actor,
      updatedOn: now,
      uniqueName: "admin",
      hasPassword: false,
      isDisabled: false,
      isConfirmed: false,
      customAttributes: [],
      customIdentifiers: [],
      roles: [],
      sessions: [],
    };
    expect(formatUser(user)).toBe("admin");
  });
});
