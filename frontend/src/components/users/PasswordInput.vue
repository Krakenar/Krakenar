<script setup lang="ts">
import type { InputType } from "logitar-vue3-ui";
import type { ValidationRuleSet } from "logitar-validation";
import { computed, ref } from "vue";
import { useI18n } from "vue-i18n";

import FormInput from "@/components/forms/FormInput.vue";
import type { PasswordSettings } from "@/types/settings";
import type { Placeholders } from "@/types/forms";

const { t } = useI18n();

const props = withDefaults(
  defineProps<{
    confirm?: string;
    id?: string;
    label?: string;
    modelValue?: string;
    required?: boolean | string;
    settings?: PasswordSettings;
    target?: string;
    type?: InputType;
  }>(),
  {
    id: "password",
    label: "users.password.label",
    type: "password",
  },
);

const inputRef = ref<InstanceType<typeof FormInput> | null>();

const placeholders = computed<Placeholders>(() => ({ target: props.target }));
const rules = computed<ValidationRuleSet>(() => ({
  confirm: props.confirm && props.target ? props.confirm : undefined,
  minimumLength: props.settings?.requiredLength,
  uniqueCharacters: props.settings?.requiredUniqueChars,
  containsUppercase: props.settings?.requireUppercase,
  containsLowercase: props.settings?.requireLowercase,
  containsDigits: props.settings?.requireDigit,
  containsNonAlphanumeric: props.settings?.requireNonAlphanumeric,
}));

defineEmits<{
  (e: "update:model-value", value: string): void;
}>();

function focus(): void {
  inputRef.value?.focus();
}
defineExpose({ focus });
</script>

<template>
  <FormInput
    :id="id"
    :label="t(label)"
    :model-value="modelValue"
    ref="inputRef"
    :placeholders="placeholders"
    :required="required"
    :rules="rules"
    :type="type"
    @update:model-value="$emit('update:model-value', $event)"
  />
</template>
