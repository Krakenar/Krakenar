<script setup lang="ts">
import { useI18n } from "vue-i18n";

import AppInput from "./AppInput.vue";
import type { UniqueNameSettings } from "@/types/settings";

const { t } = useI18n();

withDefaults(
  defineProps<{
    id?: string;
    label?: string;
    modelValue?: string;
    required?: boolean | string;
    settings?: UniqueNameSettings;
  }>(),
  {
    id: "unique-name",
    label: "uniqueName.label",
    required: true,
  },
);

// TODO(fpion): validation when settings, max+allowed_characters

defineEmits<{
  (e: "update:model-value", value: string): void;
}>();
</script>

<template>
  <AppInput :id="id" :label="t(label)" :model-value="modelValue" :required="required" @update:model-value="$emit('update:model-value', $event)" />
</template>
