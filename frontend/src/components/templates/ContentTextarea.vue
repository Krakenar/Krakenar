<script setup lang="ts">
import { useI18n } from "vue-i18n";

import FormTextarea from "@/components/forms/FormTextarea.vue";

const { t } = useI18n();

withDefaults(
  defineProps<{
    id?: string;
    label?: string;
    modelValue?: string;
    required?: boolean | string;
    rows?: number | string;
  }>(),
  {
    id: "content-text",
    label: "templates.content.text",
    rows: 15,
  },
);

defineEmits<{
  (e: "update:model-value", value: string): void;
}>();
</script>

<template>
  <FormTextarea
    :id="id"
    :label="t(label)"
    :model-value="modelValue"
    :required="required"
    :rows="rows"
    @update:model-value="$emit('update:model-value', $event)"
  />
</template>
