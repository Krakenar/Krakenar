<script setup lang="ts">
import { TarButton } from "logitar-vue3-ui";
import { ref, watch } from "vue";
import { useI18n } from "vue-i18n";

import TwilioAccountSid from "./TwilioAccountSid.vue";
import TwilioAuthenticationToken from "./TwilioAuthenticationToken.vue";
import type { Sender, UpdateSenderPayload } from "@/types/senders";
import { useForm } from "@/forms";
import { updateSender } from "@/api/senders";

const { t } = useI18n();

const props = defineProps<{
  sender: Sender;
}>();

const accountSid = ref<string>("");
const authenticationToken = ref<string>("");

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "updated", value: Sender): void;
}>();

const { hasChanges, isSubmitting, handleSubmit } = useForm();
async function submit(): Promise<void> {
  try {
    const payload: UpdateSenderPayload = {
      twilio: {
        accountSid: accountSid.value,
        authenticationToken: authenticationToken.value,
      },
    };
    const sender: Sender = await updateSender(props.sender.id, payload);
    emit("updated", sender);
  } catch (e: unknown) {
    emit("error", e);
  }
}

watch(
  props.sender,
  (sender) => {
    accountSid.value = sender.twilio?.accountSid ?? "";
    authenticationToken.value = sender.twilio?.authenticationToken ?? "";
  },
  { deep: true, immediate: true },
);
</script>

<template>
  <form @submit.prevent="handleSubmit(submit)">
    <TwilioAccountSid required v-model="accountSid" />
    <TwilioAuthenticationToken required v-model="authenticationToken" />
    <div class="mb-3">
      <TarButton
        :disabled="isSubmitting || !hasChanges"
        icon="fas fa-save"
        :loading="isSubmitting"
        :status="t('loading')"
        :text="t('actions.save')"
        type="submit"
      />
    </div>
  </form>
</template>
