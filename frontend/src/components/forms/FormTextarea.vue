<script setup lang="ts">
import type { ValidationResult, ValidationRuleSet } from "logitar-validation";
import { TarTextarea, type TextareaOptions, type TextareaStatus } from "logitar-vue3-ui";
import { computed, onUnmounted, ref } from "vue";
import { nanoid } from "nanoid";
import { parsingUtils } from "logitar-js";
import { useI18n } from "vue-i18n";

import { useField } from "@/forms";

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

defineEmits<{
  (e: "update:model-value", value: string): void;
  (e: "validated", value: ValidationResult): void;
}>();

const rules = computed<ValidationRuleSet>(() => {
  const rules: ValidationRuleSet = {
    required: parseBoolean(props.required),
    minimumLength: parseNumber(props.min),
    maximumLength: parseNumber(props.max),
  };
  return { ...rules, ...props.rules };
});
const { errors, isValid, handleChange, unbindField } = useField(props.id, {
  focus,
  initialValue: props.modelValue,
  name: props.label?.toLowerCase() ?? props.name,
  rules,
});

function focus(): void {
  textareaRef.value?.focus();
}
defineExpose({ focus });

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
