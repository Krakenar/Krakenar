<script setup lang="ts">
import type { RuleExecutionResult, ValidationResult, ValidationRuleSet } from "logitar-validation";
import { TarSelect, type SelectOptions, type SelectStatus } from "logitar-vue3-ui";
import { computed, ref } from "vue";
import { nanoid } from "nanoid";
import { parsingUtils } from "logitar-js";
import { useI18n } from "vue-i18n";

import validator, { isValidationFailure } from "@/validation";

const { parseBoolean } = parsingUtils;
const { t } = useI18n();

const props = withDefaults(defineProps<SelectOptions>(), {
  floating: true,
  id: () => nanoid(),
});

const errors = ref<RuleExecutionResult[]>([]);
const isValid = ref<boolean | undefined>();
const selectRef = ref<InstanceType<typeof TarSelect> | null>(null);

const feedbackId = computed<string>(() => `${props.id}-feedback`);
const rules = computed<ValidationRuleSet>(() => {
  const rules: ValidationRuleSet = {
    required: parseBoolean(props.required),
  };
  return rules;
});
const selectDescribedBy = computed<string>(() => [feedbackId.value, props.describedBy].filter((id) => typeof id === "string").join(" "));
const selectRequired = computed<boolean | "label">(() => (parseBoolean(props.required) ? "label" : false));
const selectStatus = computed<SelectStatus | undefined>(() => {
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
  const value: string = (e.target as HTMLSelectElement)?.value ?? "";
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
  selectRef.value?.focus();
}
defineExpose({ focus });
</script>

<template>
  <TarSelect
    :aria-label="ariaLabel"
    class="mb-3"
    :described-by="selectDescribedBy"
    :disabled="disabled"
    :floating="floating"
    :id="id"
    :label="label"
    :model-value="modelValue"
    :multiple="multiple"
    :name="name"
    :options="options"
    :placeholder="placeholder ?? label"
    ref="selectRef"
    :required="selectRequired"
    :size="size"
    :status="selectStatus"
    @blur="handleChange"
    @change="handleChange"
    @input="handleChange($event, selectStatus === 'invalid')"
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
  </TarSelect>
</template>
