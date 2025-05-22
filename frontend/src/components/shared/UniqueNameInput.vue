<script setup lang="ts">
import type { ValidationRuleSet } from "logitar-validation";
import { computed } from "vue";
import { parsingUtils } from "logitar-js";
import { useI18n } from "vue-i18n";

import FormInput from "@/components/forms/FormInput.vue";
import type { UniqueNameSettings } from "@/types/settings";

const { parseBoolean } = parsingUtils;
const { t } = useI18n();

const props = withDefaults(
  defineProps<{
    help?: string;
    id?: string;
    identifier?: boolean | string;
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
  },
);

const isIdentifier = computed<boolean>(() => parseBoolean(props.identifier) ?? false);
const rules = computed<ValidationRuleSet>(() => ({
  allowedCharacters: isIdentifier.value ? undefined : props.settings?.allowedCharacters,
  identifier: isIdentifier.value,
}));

defineEmits<{
  (e: "update:model-value", value: string): void;
}>();
</script>

<template>
  <FormInput
    :described-by="help ? `${id}-help` : undefined"
    :id="id"
    :label="t(label)"
    :max="settings ? max : undefined"
    :model-value="modelValue"
    :required="required"
    :rules="rules"
    @update:model-value="$emit('update:model-value', $event)"
  >
    <template v-if="help" #after>
      <div :id="`${id}-help`" class="form-text">{{ t(help) }}</div>
    </template>
  </FormInput>
</template>
