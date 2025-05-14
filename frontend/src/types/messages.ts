import type { Aggregate } from "./aggregate";
import type { Content, Template } from "./templates";
import type { Email, EmailPayload, Phone, PhonePayload, User } from "./users";
import type { Locale } from "./i18n";
import type { Realm } from "./realms";
import type { SearchPayload, SortOption } from "./search";
import type { Sender } from "./senders";

export type Message = Aggregate & {
  realm?: Realm | null;
  subject: string;
  body: Content;
  recipientCount: number;
  recipients: Recipient[];
  sender: Sender;
  template: Template;
  ignoreUserLocale: boolean;
  locale?: Locale | null;
  variables: Variable[];
  isDemo: boolean;
  status: MessageStatus;
  results: ResultData[];
};

export type MessageSort = "CreatedOn" | "RecipientCount" | "Subject" | "UpdatedOn";

export type MessageSortOption = SortOption & {
  field: MessageSort;
};

export type MessageStatus = "Unsent" | "Succeeded" | "Failed";

export type Recipient = {
  id: string;
  type: RecipientType;
  email?: Email | null;
  phone?: Phone | null;
  displayName?: string | null;
  user?: User | null;
};

export type RecipientPayload = {
  type: RecipientType;
  email?: EmailPayload;
  phone?: PhonePayload;
  displayName?: string;
  userId?: string;
};

export type RecipientType = "To" | "CC" | "Bcc";

export type ResultData = {
  key: string;
  value: string;
};

export type SearchMessagesPayload = SearchPayload & {
  templateId?: string;
  isDemo?: boolean;
  status?: MessageStatus;
  sort: MessageSortOption[];
};

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
