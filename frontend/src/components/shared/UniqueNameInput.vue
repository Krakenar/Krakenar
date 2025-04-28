<script setup lang="ts">
import type { ValidationRuleSet } from "logitar-validation";
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import FormInput from "@/components/forms/FormInput.vue";
import type { UniqueNameSettings } from "@/types/settings";

const { t } = useI18n();

const props = withDefaults(
  defineProps<{
    id?: string;
    label?: string;
    max?: number | string;
    modelValue?: string;
    required?: boolean | string;
    settings?: UniqueNameSettings;
  }>(),
  {
    id: "unique-name",
    label: "uniqueName.label",
    max: 255,
    required: true,
  },
);

const rules = computed<ValidationRuleSet>(() => ({ allowedCharacters: props.settings?.allowedCharacters }));

defineEmits<{
  (e: "update:model-value", value: string): void;
}>();
</script>

<template>
  <FormInput
    :id="id"
    :label="t(label)"
    :max="settings ? max : undefined"
    :model-value="modelValue"
    :required="required"
    :rules="rules"
    @update:model-value="$emit('update:model-value', $event)"
  />
</template>
