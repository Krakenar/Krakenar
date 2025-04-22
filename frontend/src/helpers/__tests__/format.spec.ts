import { describe, it, expect } from "vitest";

import type { Locale } from "@/types/i18n";
import { formatLocale } from "../format";

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
