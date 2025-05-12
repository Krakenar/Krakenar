import type { Aggregate } from "./aggregate";
import type { Change } from "./change";
import type { Realm } from "./realms";
import type { SearchPayload, SortOption } from "./search";
import type { Email, EmailPayload, Phone, PhonePayload } from "./users";

export type CreateOrReplaceSenderPayload = {
  email?: EmailPayload;
  phone?: PhonePayload;
  displayName?: string;
  description?: string;
  sendGrid?: SendGridSettings;
  twilio?: TwilioSettings;
};

export type SearchSendersPayload = SearchPayload & {
  kind?: SenderKind;
  provider?: SenderProvider;
  sort: SenderSortOption[];
};

export type Sender = Aggregate & {
  realm?: Realm | null;
  kind: SenderKind;
  isDefault: boolean;
  email?: Email | null;
  phone?: Phone | null;
  displayName?: string | null;
  description?: string | null;
  provider: SenderProvider;
  sendGrid?: SendGridSettings | null;
  twilio?: TwilioSettings | null;
};

export type SenderKind = "Email" | "Phone";

export type SenderProvider = "SendGrid" | "Twilio";

export type SenderSort = "CreatedOn" | "DisplayName" | "EmailAddress" | "PhoneNumber" | "UpdatedOn";

export type SenderSortOption = SortOption & {
  field: SenderSort;
};

export type SendGridSettings = {
  apiKey: string;
};

export type TwilioSettings = {
  accountSid: string;
  authenticationToken: string;
};

export type UpdateSenderPayload = {
  email?: EmailPayload;
  phone?: PhonePayload;
  displayName?: Change<string>;
  description?: Change<string>;
  sendGrid?: SendGridSettings;
  twilio?: TwilioSettings;
};
