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
  }>(),
  {
    id: "postal-code",
    label: "users.address.postalCode",
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
    :pattern="country?.postalCode || undefined"
    :required="required"
    @update:model-value="$emit('update:model-value', $event)"
  />
</template>
