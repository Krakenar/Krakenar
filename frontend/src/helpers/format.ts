import type { Locale } from "@/types/i18n";

export function formatLocale(locale: Locale): string {
  return `${locale.displayName} (${locale.code})`;
}
