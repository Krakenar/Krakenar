<script setup lang="ts">
import { useI18n } from "vue-i18n";

import AppTextarea from "./AppTextarea.vue";

const { t } = useI18n();

withDefaults(
  defineProps<{
    id?: string;
    label?: string;
    modelValue?: string;
    rows?: number | string;
  }>(),
  {
    id: "description",
    label: "description",
    rows: 15,
  },
);

defineEmits<{
  (e: "update:model-value", value: string): void;
}>();
</script>

<template>
  <AppTextarea :id="id" :label="t(label)" :model-value="modelValue" :rows="rows" @update:model-value="$emit('update:model-value', $event)" />
</template>
