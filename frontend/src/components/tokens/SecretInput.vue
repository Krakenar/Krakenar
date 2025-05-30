<script setup lang="ts">
import { useI18n } from "vue-i18n";

import FormInput from "@/components/forms/FormInput.vue";

const { t } = useI18n();

withDefaults(
  defineProps<{
    id?: string;
    label?: string;
    max?: string | number;
    min?: number | string;
    modelValue?: string;
    placeholder?: string;
    required?: boolean | string;
  }>(),
  {
    id: "secret",
    label: "tokens.secret.label",
    max: 512 / 8,
    min: 256 / 8,
  },
);

defineEmits<{
  (e: "update:model-value", value: string): void;
}>();
</script>

<template>
  <FormInput
    :id="id"
    :label="t(label)"
    :max="max"
    :min="min"
    :model-value="modelValue"
    :placeholder="placeholder ? t(placeholder) : undefined"
    :required="required"
    @update:model-value="$emit('update:model-value', $event)"
  />
</template>
