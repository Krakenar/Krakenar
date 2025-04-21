<script setup lang="ts">
import { useI18n } from "vue-i18n";

import AppInput from "./AppInput.vue";

const { t } = useI18n();

withDefaults(
  defineProps<{
    id?: string;
    label?: string;
    modelValue?: string;
    required?: boolean | string;
  }>(),
  {
    id: "unique-name",
    label: "uniqueName",
    required: true,
  },
);

// TODO(fpion): validation when settings

defineEmits<{
  (e: "update:model-value", value: string): void;
}>();
</script>

<template>
  <AppInput :id="id" :label="t(label)" :model-value="modelValue" :required="required" @update:model-value="$emit('update:model-value', $event)" />
</template>
