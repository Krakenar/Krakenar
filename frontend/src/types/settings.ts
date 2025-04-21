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
  allowedCharacters?: string;
};
