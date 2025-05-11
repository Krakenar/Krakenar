import { describe, it, expect } from "vitest";
import { nanoid } from "nanoid";

import type { Actor } from "@/types/actor";
import type { Locale } from "@/types/i18n";
import type { Role } from "@/types/roles";
import type { Template } from "@/types/templates";
import type { User } from "@/types/users";
import { formatLocale, formatRole, formatTemplate, formatUser } from "../format";

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

const actor: Actor = {
  type: "User",
  id: nanoid(),
  isDeleted: false,
  displayName: "Administrator",
};
const now: string = new Date().toISOString();

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
      content: { type: "plain/text", text: "Hello World!" },
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
      content: { type: "plain/text", text: "Hello World!" },
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
