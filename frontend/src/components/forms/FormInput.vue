<script setup lang="ts">
import type { ValidationResult, ValidationRuleSet } from "logitar-validation";
import { TarInput, type InputOptions, type InputStatus } from "logitar-vue3-ui";
import { computed, onUnmounted, ref } from "vue";
import { nanoid } from "nanoid";
import { parsingUtils } from "logitar-js";
import { useI18n } from "vue-i18n";

import { useField } from "@/forms";

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
    minimumLength: parseNumber(props.min),
    maximumLength: parseNumber(props.max),
    email: props.type === "email",
  };
  return { ...rules, ...props.rules };
});
const { errors, isValid, value, handleChange, unbindField } = useField(props.id, {
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
    :model-value="value"
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
