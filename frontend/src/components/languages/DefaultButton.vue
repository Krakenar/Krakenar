<script setup lang="ts">
import { TarButton } from "logitar-vue3-ui";
import { ref } from "vue";
import { useI18n } from "vue-i18n";

import type { Language } from "@/types/languages";
import { setDefaultLanguage } from "@/api/languages";

const { t } = useI18n();

const props = defineProps<{
  language: Language;
}>();

const isLoading = ref<boolean>(false);

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "saved", value: Language): void;
}>();

async function onClick(): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    try {
      const language: Language = await setDefaultLanguage(props.language.id);
      emit("saved", language);
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
    :disabled="language.isDefault"
    :icon="language.isDefault ? 'fas fa-check' : 'fas fa-star'"
    :loading="isLoading"
    :text="t('languages.default.label')"
    :variant="language.isDefault ? 'info' : 'warning'"
    @click="onClick"
  />
</template>
