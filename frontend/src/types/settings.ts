export type LoggingExtent = "None" | "ActivityOnly" | "Full";

export type LoggingSettings = {
  extent: LoggingExtent;
  onlyErrors: boolean;
};

export type PasswordSettings = {
  requiredLength: number;
  requiredUniqueChars: number;
  requireNonAlphanumeric: boolean;
  requireLowercase: boolean;
  requireUppercase: boolean;
  requireDigit: boolean;
  hashingStrategy: string;
};

export type UniqueNameSettings = {
  allowedCharacters?: string | null;
};
