<script setup lang="ts">
import { useI18n } from "vue-i18n";

import AppInput from "./AppInput.vue";

const { t } = useI18n();

withDefaults(
  defineProps<{
    id?: string;
    label?: string;
    max?: number | string;
    modelValue?: string;
    required?: boolean | string;
  }>(),
  {
    id: "display-name",
    label: "displayName",
    max: 255,
  },
);

defineEmits<{
  (e: "update:model-value", value: string): void;
}>();
</script>

<template>
  <AppInput :id="id" :label="t(label)" :max="max" :model-value="modelValue" :required="required" @update:model-value="$emit('update:model-value', $event)" />
</template>
