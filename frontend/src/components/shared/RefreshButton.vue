<script setup lang="ts">
import { TarButton } from "logitar-vue3-ui";
import { useI18n } from "vue-i18n";

const { t } = useI18n();

withDefaults(
  defineProps<{
    disabled?: boolean | string;
    icon?: string;
    loading?: boolean | string;
    status?: string;
    text?: string;
  }>(),
  {
    icon: "fas fa-rotate",
    status: "loading",
    text: "actions.refresh",
  },
);

defineEmits<{
  (e: "click"): void;
}>();
</script>

<template>
  <TarButton :disabled="disabled" :icon="icon" :loading="loading" :status="t(status)" :text="t(text)" @click="$emit('click')" />
</template>
