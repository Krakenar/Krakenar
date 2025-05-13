import type { EmailPayload, PhonePayload } from "./users";

export type RecipientPayload = {
  type: RecipientType;
  email?: EmailPayload;
  phone?: PhonePayload;
  displayName?: string;
  userId?: string;
};

export type RecipientType = "To" | "CC" | "Bcc";

export type SendMessagePayload = {
  sender: string;
  template: string;
  recipients: RecipientPayload[];
  ignoreUserLocale: boolean;
  locale?: string;
  variables: Variable[];
  isDemo: boolean;
};

export type SentMessages = {
  ids: string[];
};

export type Variable = {
  key: string;
  value: string;
};
