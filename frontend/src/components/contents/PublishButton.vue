<script setup lang="ts">
import { TarButton, type ButtonVariant } from "logitar-vue3-ui";
import { computed, ref } from "vue";
import { useI18n } from "vue-i18n";

import type { Content } from "@/types/contents";
import { publishAllContent } from "@/api/contents/items";

const { t } = useI18n();

const props = withDefaults(
  defineProps<{
    content: Content;
    icon?: string;
    text?: string;
    variant?: ButtonVariant;
  }>(),
  {
    icon: "fas fa-bullhorn",
    text: "actions.publish",
    variant: "warning",
  },
);

const isLoading = ref<boolean>(false);

const isDisabled = computed<boolean>(() => props.content.invariant.isPublished && props.content.locales.every((locale) => locale.isPublished));

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "published", value: Content): void;
}>();

async function onClick(): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    try {
      const content: Content = await publishAllContent(props.content.id);
      emit("published", content);
    } catch (e: unknown) {
      emit("error", e);
    } finally {
      isLoading.value = false;
    }
  }
}
</script>

<template>
  <TarButton :disabled="isDisabled" :icon="icon" :loading="isLoading" :text="t(text)" :variant="variant" @click="onClick" />
</template>
