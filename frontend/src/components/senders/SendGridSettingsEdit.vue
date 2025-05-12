<script setup lang="ts">
import { TarButton } from "logitar-vue3-ui";
import { ref, watch } from "vue";
import { useI18n } from "vue-i18n";

import SendGridApiKey from "./SendGridApiKey.vue";
import type { Sender, UpdateSenderPayload } from "@/types/senders";
import { useForm } from "@/forms";
import { updateSender } from "@/api/senders";

const { t } = useI18n();

const props = defineProps<{
  sender: Sender;
}>();

const apiKey = ref<string>("");

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "updated", value: Sender): void;
}>();

const { hasChanges, isSubmitting, handleSubmit } = useForm();
async function submit(): Promise<void> {
  try {
    const payload: UpdateSenderPayload = {
      sendGrid: {
        apiKey: apiKey.value,
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
    apiKey.value = sender.sendGrid?.apiKey ?? "";
  },
  { deep: true, immediate: true },
);
</script>

<template>
  <form @submit.prevent="handleSubmit(submit)">
    <SendGridApiKey required v-model="apiKey" />
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
