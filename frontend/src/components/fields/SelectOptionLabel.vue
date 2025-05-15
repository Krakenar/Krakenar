<script setup lang="ts">
import { useI18n } from "vue-i18n";

import FormInput from "@/components/forms/FormInput.vue";

const { t } = useI18n();

withDefaults(
  defineProps<{
    id?: string;
    label?: string;
    modelValue?: string;
    required?: boolean | string;
  }>(),
  {
    id: "label",
    label: "fields.type.select.option.label",
  },
);

defineEmits<{
  (e: "update:model-value", value: string): void;
}>();
</script>

<template>
  <FormInput :id="id" :label="t(label)" :model-value="modelValue" :required="required" @update:model-value="$emit('update:model-value', $event)">
    <template #append>
      <slot name="append"></slot>
    </template>
  </FormInput>
</template>
