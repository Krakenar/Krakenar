<script setup lang="ts">
import { TarButton, TarCheckbox } from "logitar-vue3-ui";
import { computed, ref } from "vue";
import { useI18n } from "vue-i18n";

import LocaleSelect from "@/components/shared/LocaleSelect.vue";
import SenderSelect from "@/components/senders/SenderSelect.vue";
import SentMessage from "./SentMessage.vue";
import TemplateSelect from "@/components/templates/TemplateSelect.vue";
import VariableList from "./VariableList.vue";
import locales from "@/resources/locales.json";
import type { ContentType, Template } from "@/types/templates";
import type { Locale } from "@/types/i18n";
import type { SendMessagePayload, SentMessages, Variable } from "@/types/messages";
import type { Sender, SenderKind } from "@/types/senders";
import { sendMessage } from "@/api/messages";
import { useAccountStore } from "@/stores/account";
import { useForm } from "@/forms";

const account = useAccountStore();
const { locale, t } = useI18n();

const props = defineProps<{
  sender?: Sender;
  template?: Template;
}>();
if ((!props.sender && !props.template) || (props.sender && props.template)) {
  throw new Error("Exactly one of the following properties must be specified: sender, template.");
}

const ignoreUserLocale = ref<boolean>(false);
const selectedLocale = ref<Locale | undefined>(locales.find(({ code }) => code === locale.value));
const selectedSender = ref<Sender>();
const selectedTemplate = ref<Template>();
const sentMessageId = ref<string>("");
const variables = ref<Variable[]>([]);

const contentType = computed<ContentType | undefined>(() => (props.sender?.kind === "Phone" ? "text/plain" : undefined));
const kind = computed<SenderKind | undefined>(() => (props.template?.content.type === "text/html" ? "Email" : undefined));

const emit = defineEmits<{
  (e: "error", value: unknown): void;
}>();

const { isSubmitting, handleSubmit } = useForm();
async function submit(): Promise<void> {
  try {
    sentMessageId.value = "";
    const payload: SendMessagePayload = {
      sender: props.sender?.id ?? selectedSender.value?.id ?? "",
      template: props.template?.id ?? selectedTemplate.value?.id ?? "",
      recipients: [{ type: "To", userId: account.currentUser?.id }], // TODO(fpion): will not work inside a realm
      ignoreUserLocale: ignoreUserLocale.value,
      locale: selectedLocale.value?.code,
      variables: variables.value,
      isDemo: true,
    };
    const sentMessages: SentMessages = await sendMessage(payload);
    if (sentMessages.ids.length > 0) {
      sentMessageId.value = sentMessages.ids[0];
    }
  } catch (e: unknown) {
    emit("error", e);
  }
}
</script>

<template>
  <div>
    <SentMessage v-if="sentMessageId" :id="sentMessageId" />
    <form @submit.prevent="handleSubmit(submit)">
      <SenderSelect v-if="!sender" :kind="kind" :model-value="selectedSender?.id" required @selected="selectedSender = $event" />
      <TemplateSelect v-if="!template" :content-type="contentType" :model-value="selectedTemplate?.id" required @selected="selectedTemplate = $event" />
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
