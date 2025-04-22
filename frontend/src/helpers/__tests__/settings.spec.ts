import { describe, it, expect } from "vitest";

import { arePasswordEqual, areUniqueNameEqual } from "../settings";
import type { PasswordSettings, UniqueNameSettings } from "@/types/settings";

describe("arePasswordEqual", () => {
  it.concurrent("should return false when the /settings differ", () => {
    const a: PasswordSettings = {
      requiredLength: 6,
      requiredUniqueChars: 1,
      requireNonAlphanumeric: false,
      requireLowercase: true,
      requireUppercase: true,
      requireDigit: true,
      hashingStrategy: "PBKDF2",
    };
    const b: PasswordSettings = {
      requiredLength: 8,
      requiredUniqueChars: 8,
      requireNonAlphanumeric: true,
      requireLowercase: true,
      requireUppercase: true,
      requireDigit: true,
      hashingStrategy: "PBKDF2",
    };
    expect(arePasswordEqual(a, b)).toBe(false);
  });

  it.concurrent("should return true when the settings are equal", () => {
    const a: PasswordSettings = {
      requiredLength: 8,
      requiredUniqueChars: 8,
      requireNonAlphanumeric: true,
      requireLowercase: true,
      requireUppercase: true,
      requireDigit: true,
      hashingStrategy: "PBKDF2",
    };
    const b: PasswordSettings = {
      requiredLength: 8,
      requiredUniqueChars: 8,
      requireNonAlphanumeric: true,
      requireLowercase: true,
      requireUppercase: true,
      requireDigit: true,
      hashingStrategy: "PBKDF2",
    };
    expect(arePasswordEqual(a, b)).toBe(true);
  });
});

describe("areUniqueNameEqual", () => {
  it.concurrent("should return false when the settings differ", () => {
    const a: UniqueNameSettings = {};
    const b: UniqueNameSettings = { allowedCharacters: "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+" };
    expect(areUniqueNameEqual(a, b)).toBe(false);
  });

  it.concurrent("should return true when the settings are equal", () => {
    const a: UniqueNameSettings = { allowedCharacters: "0123456789" };
    const b: UniqueNameSettings = { allowedCharacters: "0123456789" };
    expect(areUniqueNameEqual(a, b)).toBe(true);
  });
});
