<script setup lang="ts">
import { TarButton, TarCheckbox } from "logitar-vue3-ui";
import { computed, ref } from "vue";
import { useI18n } from "vue-i18n";

import LocaleSelect from "@/components/shared/LocaleSelect.vue";
import MissingRecipientContacts from "./MissingRecipientContacts.vue";
import SenderSelect from "@/components/senders/SenderSelect.vue";
import SentMessage from "./SentMessage.vue";
import TemplateFormSelect from "@/components/templates/TemplateFormSelect.vue";
import VariableList from "./VariableList.vue";
import locales from "@/resources/locales.json";
import type { CurrentUser } from "@/types/account";
import type { Locale } from "@/types/i18n";
import type { MediaType } from "@/types/contents";
import type { SendMessagePayload, SentMessages, Variable } from "@/types/messages";
import type { Sender, SenderKind } from "@/types/senders";
import type { Template } from "@/types/templates";
import { ErrorCodes, StatusCodes } from "@/types/api";
import { isError } from "@/helpers/error";
import { sendMessage } from "@/api/messages";
import { useAccountStore } from "@/stores/account";
import { useForm } from "@/forms";
import { useRealmStore } from "@/stores/realm";

const account = useAccountStore();
const realm = useRealmStore();
const { locale, t } = useI18n();

const props = defineProps<{
  sender?: Sender;
  template?: Template;
}>();
if ((!props.sender && !props.template) || (props.sender && props.template)) {
  throw new Error("Exactly one of the following properties must be specified: sender, template.");
}

const ignoreUserLocale = ref<boolean>(false);
const missingRecipientContacts = ref<boolean>(false);
const selectedLocale = ref<Locale | undefined>(locales.find(({ code }) => code === locale.value));
const selectedSender = ref<Sender>();
const selectedTemplate = ref<Template>();
const sentMessageId = ref<string>("");
const variables = ref<Variable[]>([]);

const contentType = computed<MediaType | undefined>(() => (props.sender?.kind === "Phone" ? "text/plain" : undefined));
const kind = computed<SenderKind | undefined>(() => (props.template?.content.type === "text/html" ? "Email" : undefined));

const emit = defineEmits<{
  (e: "error", value: unknown): void;
}>();

const { isSubmitting, handleSubmit } = useForm();
async function submit(): Promise<void> {
  try {
    missingRecipientContacts.value = false;
    sentMessageId.value = "";
    const sender: Sender | undefined = props.sender ?? selectedSender.value;
    const template: Template | undefined = props.template ?? selectedTemplate.value;
    const user: CurrentUser | undefined = account.currentUser;
    if (sender && template && user) {
      const payload: SendMessagePayload = {
        sender: sender.id,
        template: template.id,
        recipients: [],
        ignoreUserLocale: ignoreUserLocale.value,
        locale: selectedLocale.value?.code,
        variables: variables.value,
        isDemo: true,
      };
      if (realm.currentRealm) {
        payload.recipients.push({
          type: "To",
          email: sender.kind === "Email" && user.emailAddress ? { address: user.emailAddress, isVerified: false } : undefined,
          phone: sender.kind === "Phone" && user.phoneNumber ? { number: user.phoneNumber, isVerified: false } : undefined,
          displayName: user.displayName,
        });
      } else {
        payload.recipients.push({ type: "To", userId: user.id });
      }
      const sentMessages: SentMessages = await sendMessage(payload);
      if (sentMessages.ids.length > 0) {
        sentMessageId.value = sentMessages.ids[0];
      }
    }
  } catch (e: unknown) {
    if (isError(e, StatusCodes.BadRequest, ErrorCodes.MissingRecipientContacts)) {
      missingRecipientContacts.value = true;
    } else {
      emit("error", e);
    }
  }
}
</script>

<template>
  <div>
    <MissingRecipientContacts v-model="missingRecipientContacts" />
    <SentMessage v-if="sentMessageId" :id="sentMessageId" />
    <form @submit.prevent="handleSubmit(submit)">
      <SenderSelect v-if="!sender" :kind="kind" :model-value="selectedSender?.id" required @selected="selectedSender = $event" />
      <TemplateFormSelect v-if="!template" :content-type="contentType" :model-value="selectedTemplate?.id" required @selected="selectedTemplate = $event" />
      <LocaleSelect :model-value="selectedLocale?.code" :required="ignoreUserLocale" @selected="selectedLocale = $event">
        <template #after>
          <TarCheckbox id="ignore-user-locale" :label="t('messages.ignoreUserLocale')" v-model="ignoreUserLocale" />
        </template>
      </LocaleSelect>
      <VariableList v-model="variables" />
      <div class="mb-3">
        <TarButton :disabled="isSubmitting" icon="fas fa-paper-plane" :loading="isSubmitting" :status="t('loading')" :text="t('actions.send')" type="submit" />
      </div>
    </form>
  </div>
</template>
