<script setup lang="ts">
import type { RuleExecutionResult, ValidationResult, ValidationRuleSet } from "logitar-validation";
import { TarInput, inputUtils, type InputOptions, type InputStatus } from "logitar-vue3-ui";
import { computed, ref } from "vue";
import { nanoid } from "nanoid";
import { parsingUtils } from "logitar-js";
import { useI18n } from "vue-i18n";

import validator from "@/validation";

const { isDateTimeInput } = inputUtils;
const { parseBoolean, parseNumber } = parsingUtils;
const { t } = useI18n();

const props = withDefaults(defineProps<InputOptions>(), {
  floating: true,
  id: () => nanoid(),
});

const error = ref<RuleExecutionResult>();
const inputRef = ref<InstanceType<typeof TarInput> | null>(null);
const isValid = ref<boolean | undefined>();

const feedbackId = computed<string>(() => `${props.id}-feedback`);
const inputDescribedBy = computed<string>(() => [feedbackId.value, props.describedBy].filter((id) => typeof id === "string").join(" "));
const inputMax = computed<number | string | undefined>(() => (isDateTimeInput(props.type) ? props.max : undefined));
const inputMin = computed<number | string | undefined>(() => (isDateTimeInput(props.type) ? props.min : undefined));
const inputRequired = computed<boolean | "label">(() => (parseBoolean(props.required) ? "label" : false));
const inputStatus = computed<InputStatus | undefined>(() => {
  if (props.status) {
    return props.status;
  }
  switch (isValid.value) {
    case false:
      return "invalid";
    case true:
      return "valid";
  }
  return undefined;
});
const rules = computed<ValidationRuleSet>(() => {
  const rules: ValidationRuleSet = {
    required: parseBoolean(props.required),
    minimumLength: parseNumber(props.min),
    maximumLength: parseNumber(props.max),
    email: props.type === "email",
  };
  return rules;
});

const emit = defineEmits<{
  (e: "update:model-value", value: string): void;
  (e: "validated", value: ValidationResult): void;
}>();

function isError(result: RuleExecutionResult): boolean {
  switch (result.severity) {
    case "warning":
    case "error":
    case "critical":
      return true;
  }
  return false;
}
function handleChange(e: Event, validate: boolean = true): void {
  const value: string = (e.target as HTMLInputElement)?.value ?? "";
  emit("update:model-value", value);
  if (validate) {
    const name: string = props.label?.toLowerCase() ?? props.name ?? props.id;
    const result: ValidationResult = validator.validate(name, value, rules.value);
    isValid.value = result.isValid;
    error.value = Object.values(result.rules).find(isError);
    emit("validated", result);
  }
}

function focus(): void {
  inputRef.value?.focus();
}
defineExpose({ focus });
</script>

<template>
  <TarInput
    class="mb-3"
    :described-by="inputDescribedBy"
    :disabled="disabled"
    :floating="floating"
    :id="id"
    :label="label"
    :max="inputMax"
    :min="inputMin"
    :model-value="modelValue"
    :name="name"
    :placeholder="placeholder ?? label"
    :plaintext="plaintext"
    :readonly="readonly"
    ref="inputRef"
    :required="inputRequired"
    :size="size"
    :status="inputStatus"
    :step="step"
    :type="type"
    @blur="handleChange"
    @change="handleChange"
    @input="handleChange($event, inputStatus === 'invalid')"
  >
    <template #before>
      <slot name="before"></slot>
    </template>
    <template #prepend>
      <slot name="prepend"></slot>
    </template>
    <template #label-override>
      <slot name="label-override"></slot>
    </template>
    <template #label-required>
      <slot name="label-required"></slot>
    </template>
    <template #append>
      <slot name="append"></slot>
    </template>
    <template #after>
      <div v-if="error" class="invalid-feedback" :id="feedbackId">
        {{ t(`errors.${error.key}`, error.placeholders) }}
      </div>
      <slot name="after"></slot>
    </template>
  </TarInput>
</template>
