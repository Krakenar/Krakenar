<script setup lang="ts">
import { TarButton, type ButtonVariant } from "logitar-vue3-ui";
import { computed, ref } from "vue";
import { parsingUtils } from "logitar-js";
import { useI18n } from "vue-i18n";

import type { Content, ContentLocale } from "@/types/contents";
import type { Language } from "@/types/languages";
import { unpublishAllContent, unpublishContent } from "@/api/contents/items";

const { parseBoolean } = parsingUtils;
const { t } = useI18n();

const props = withDefaults(
  defineProps<{
    all?: boolean | string;
    content: Content;
    icon?: string;
    language?: Language;
    text?: string;
    variant?: ButtonVariant;
  }>(),
  {
    icon: "fas fa-mask",
    text: "actions.unpublish",
    variant: "warning",
  },
);

const isLoading = ref<boolean>(false);

const isAll = computed<boolean>(() => parseBoolean(props.all) ?? false);
const isDisabled = computed<boolean>(() => {
  if (isAll.value) {
    return !props.content.invariant.isPublished && props.content.locales.every((locale) => !locale.isPublished);
  }
  const locale: ContentLocale =
    (props.language ? props.content.locales.find((locale) => locale.language?.id === props.language?.id) : undefined) ?? props.content.invariant;
  return !locale.isPublished;
});

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "unpublished", value: Content): void;
}>();

async function onClick(): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    try {
      const content: Content = isAll.value ? await unpublishAllContent(props.content.id) : await unpublishContent(props.content.id, props.language?.id);
      emit("unpublished", content);
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
