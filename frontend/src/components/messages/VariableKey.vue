<script setup lang="ts">
import { useI18n } from "vue-i18n";

import FormInput from "@/components/forms/FormInput.vue";

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
    id: "name",
    label: "messages.variables.key",
    max: 255,
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
    :model-value="modelValue"
    :required="required"
    :rules="{ identifier: true }"
    @update:model-value="$emit('update:model-value', $event)"
  >
    <template #append>
      <slot name="append"></slot>
    </template>
  </FormInput>
</template>
