import type { PasswordSettings, UniqueNameSettings } from "@/types/settings";

export function arePasswordEqual(a: PasswordSettings, b: PasswordSettings): boolean {
  return (
    a.requiredLength === b.requiredLength &&
    a.requiredUniqueChars === b.requiredUniqueChars &&
    a.requireNonAlphanumeric === b.requireNonAlphanumeric &&
    a.requireLowercase === b.requireLowercase &&
    a.requireUppercase === b.requireUppercase &&
    a.requireDigit === b.requireDigit &&
    a.hashingStrategy === b.hashingStrategy
  );
}

export function areUniqueNameEqual(a: UniqueNameSettings, b: UniqueNameSettings): boolean {
  return (a.allowedCharacters ?? "") === (b.allowedCharacters ?? "");
}
