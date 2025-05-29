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
    id: "unique-slug",
    label: "realms.uniqueSlug.label",
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
    :rules="{ slug: true }"
    @update:model-value="$emit('update:model-value', $event)"
  />
</template>
