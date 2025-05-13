<script setup lang="ts">
import { useI18n } from "vue-i18n";

import FormInput from "@/components/forms/FormInput.vue";
import type { Country } from "@/types/users";

const { t } = useI18n();

withDefaults(
  defineProps<{
    country?: Country;
    id?: string;
    label?: string;
    max?: number | string;
    modelValue?: string;
    required?: boolean | string;
    verified?: boolean | string;
  }>(),
  {
    id: "phone-number",
    label: "users.phone.number",
    max: 20,
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
    :rules="{ phone: country?.code }"
    @update:model-value="$emit('update:model-value', $event)"
  >
    <template #append>
      <slot name="append"></slot>
    </template>
  </FormInput>
</template>
