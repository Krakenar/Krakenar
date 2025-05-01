<script setup lang="ts">
import type { ValidationResult, ValidationRuleSet } from "logitar-validation";
import { TarInput, inputUtils, type InputOptions, type InputStatus } from "logitar-vue3-ui";
import { computed, onUnmounted, ref } from "vue";
import { nanoid } from "nanoid";
import { parsingUtils } from "logitar-js";
import { useI18n } from "vue-i18n";

import { useField } from "@/forms";

const { isDateTimeInput, isNumericInput, isTextualInput } = inputUtils;
const { parseBoolean, parseNumber } = parsingUtils;
const { t } = useI18n();

const props = withDefaults(
  defineProps<
    InputOptions & {
      rules?: ValidationRuleSet;
    }
  >(),
  {
    floating: true,
    id: () => nanoid(),
  },
);

const inputRef = ref<InstanceType<typeof TarInput> | null>(null);

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

defineEmits<{
  (e: "update:model-value", value: string): void;
  (e: "validated", value: ValidationResult): void;
}>();

const rules = computed<ValidationRuleSet>(() => {
  const rules: ValidationRuleSet = {
    required: parseBoolean(props.required),
  };
  if (isTextualInput(props.type)) {
    rules.minimumLength = parseNumber(props.min);
    rules.maximumLength = parseNumber(props.max);
    rules.pattern = props.pattern || undefined;
  } else if (isNumericInput(props.type)) {
    rules.minimumValue = parseNumber(props.min);
    rules.maximumValue = parseNumber(props.max);
  }
  switch (props.type) {
    case "email":
      rules.email = true;
      break;
    case "url":
      rules.url = true;
      break;
  }
  return { ...rules, ...props.rules };
});
const { errors, isValid, handleChange, unbindField } = useField(props.id, {
  focus,
  initialValue: props.modelValue,
  name: props.label?.toLowerCase() ?? props.name,
  rules,
});

function focus(): void {
  inputRef.value?.focus();
}
defineExpose({ focus });

onUnmounted(() => {
  if (unbindField) {
    unbindField(props.id);
  }
});
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
    @input="handleChange($event, inputStatus !== 'invalid')"
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
      <div v-if="errors.length" class="invalid-feedback" :id="feedbackId">
        {{ t(`errors.${errors[0].key}`, errors[0].placeholders) }}
      </div>
      <slot name="after"></slot>
    </template>
  </TarInput>
</template>
