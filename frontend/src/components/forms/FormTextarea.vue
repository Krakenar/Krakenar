<script setup lang="ts">
import type { RuleExecutionResult, ValidationResult, ValidationRuleSet } from "logitar-validation";
import { TarTextarea, type TextareaOptions, type TextareaStatus } from "logitar-vue3-ui";
import { computed, inject, onMounted, onUnmounted, ref } from "vue";
import { nanoid } from "nanoid";
import { parsingUtils } from "logitar-js";
import { useI18n } from "vue-i18n";

import validator, { isValidationFailure } from "@/validation";
import { bindFieldKey, unbindFieldKey, type FieldOptions } from "@/types/forms";

const bindField: ((id: string, options: FieldOptions) => void) | undefined = inject(bindFieldKey);
const unbindField: ((id: string) => void) | undefined = inject(unbindFieldKey);
const { parseBoolean, parseNumber } = parsingUtils;
const { t } = useI18n();

const props = withDefaults(
  defineProps<
    TextareaOptions & {
      rules?: ValidationRuleSet;
    }
  >(),
  {
    floating: true,
    id: () => nanoid(),
  },
);

const errors = ref<RuleExecutionResult[]>([]);
const initialValue = ref<string>(props.modelValue ?? "");
const isValid = ref<boolean | undefined>();
const textareaRef = ref<InstanceType<typeof TarTextarea> | null>(null);

const feedbackId = computed<string>(() => `${props.id}-feedback`);
const textareaDescribedBy = computed<string>(() => [feedbackId.value, props.describedBy].filter((id) => typeof id === "string").join(" "));
const textareaRequired = computed<boolean | "label">(() => (parseBoolean(props.required) ? "label" : false));
const textareaStatus = computed<TextareaStatus | undefined>(() => {
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
const validationRules = computed<ValidationRuleSet>(() => {
  const rules: ValidationRuleSet = {
    required: parseBoolean(props.required),
    minimumLength: parseNumber(props.min),
    maximumLength: parseNumber(props.max),
  };
  return { ...rules, ...props.rules };
});

const emit = defineEmits<{
  (e: "update:model-value", value: string): void;
  (e: "validated", value: ValidationResult): void;
}>();

function handleChange(e: Event, shouldValidate: boolean = true): void {
  const value: string = (e.target as HTMLTextAreaElement)?.value ?? "";
  emit("update:model-value", value);
  if (shouldValidate) {
    validate(value);
  }
}

function focus(): void {
  textareaRef.value?.focus();
}
function reinitialize(): void {
  errors.value = [];
  initialValue.value = props.modelValue ?? "";
  isValid.value = undefined;
}
function reset(): void {
  errors.value = [];
  isValid.value = undefined;
  emit("update:model-value", initialValue.value);
}
function validate(value?: string): ValidationResult {
  const name: string = props.label?.toLowerCase() ?? props.name ?? props.id;
  value ??= props.modelValue;
  const result: ValidationResult = validator.validate(name, value, validationRules.value);
  isValid.value = result.isValid;
  errors.value = Object.values(result.rules).filter(isValidationFailure);
  emit("validated", result);
  return result;
}
defineExpose({ focus, reinitialize, reset, validate });

const fieldOptions: FieldOptions = { focus, reinitialize, reset, validate };
onMounted(() => {
  if (bindField) {
    bindField(props.id, fieldOptions);
  }
});
onUnmounted(() => {
  if (unbindField) {
    unbindField(props.id);
  }
});
</script>

<template>
  <TarTextarea
    class="mb-3"
    :cols="cols"
    :described-by="textareaDescribedBy"
    :disabled="disabled"
    :floating="floating"
    :id="id"
    :label="label"
    :model-value="modelValue"
    :name="name"
    :placeholder="placeholder ?? label"
    :plaintext="plaintext"
    :readonly="readonly"
    ref="textareaRef"
    :required="textareaRequired"
    :rows="rows"
    :size="size"
    :status="textareaStatus"
    @blur="handleChange"
    @change="handleChange"
    @input="handleChange($event, textareaStatus === 'invalid')"
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
  </TarTextarea>
</template>
