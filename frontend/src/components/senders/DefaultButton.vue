<script setup lang="ts">
import { TarButton } from "logitar-vue3-ui";
import { ref } from "vue";
import { useI18n } from "vue-i18n";

import type { Sender } from "@/types/senders";
import { setDefaultSender } from "@/api/senders";

const { t } = useI18n();

const props = defineProps<{
  sender: Sender;
}>();

const isLoading = ref<boolean>(false);

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "saved", value: Sender): void;
}>();

async function onClick(): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    try {
      const sender: Sender = await setDefaultSender(props.sender.id);
      emit("saved", sender);
    } catch (e: unknown) {
      emit("error", e);
    } finally {
      isLoading.value = false;
    }
  }
}
</script>

<template>
  <TarButton
    :disabled="sender.isDefault"
    :icon="sender.isDefault ? 'fas fa-check' : 'fas fa-star'"
    :loading="isLoading"
    :status="t('loading')"
    :text="t('senders.default.label')"
    :variant="sender.isDefault ? 'info' : 'warning'"
    @click="onClick"
  />
</template>
