<script setup lang="ts">
import type { RuleExecutionResult, ValidationResult, ValidationRuleSet } from "logitar-validation";
import { TarTextarea, type TextareaOptions, type TextareaStatus } from "logitar-vue3-ui";
import { computed, ref } from "vue";
import { nanoid } from "nanoid";
import { parsingUtils } from "logitar-js";
import { useI18n } from "vue-i18n";

import validator, { isValidationFailure } from "@/validation";

const { parseBoolean, parseNumber } = parsingUtils;
const { t } = useI18n();

const props = withDefaults(defineProps<TextareaOptions>(), {
  floating: true,
  id: () => nanoid(),
});

const errors = ref<RuleExecutionResult[]>([]);
const isValid = ref<boolean | undefined>();
const textareaRef = ref<InstanceType<typeof TarTextarea> | null>(null);

const feedbackId = computed<string>(() => `${props.id}-feedback`);
const rules = computed<ValidationRuleSet>(() => {
  const rules: ValidationRuleSet = {
    required: parseBoolean(props.required),
    minimumLength: parseNumber(props.min),
    maximumLength: parseNumber(props.max),
  };
  return rules;
});
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

const emit = defineEmits<{
  (e: "update:model-value", value: string): void;
  (e: "validated", value: ValidationResult): void;
}>();

function handleChange(e: Event, validate: boolean = true): void {
  const value: string = (e.target as HTMLTextAreaElement)?.value ?? "";
  emit("update:model-value", value);
  if (validate) {
    const name: string = props.label?.toLowerCase() ?? props.name ?? props.id;
    const result: ValidationResult = validator.validate(name, value, rules.value);
    isValid.value = result.isValid;
    errors.value = Object.values(result.rules).filter(isValidationFailure);
    emit("validated", result);
  }
}

function focus(): void {
  textareaRef.value?.focus();
}
defineExpose({ focus });
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
