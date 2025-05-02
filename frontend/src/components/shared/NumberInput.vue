<script setup lang="ts">
import type { InputSize, InputStatus, InputType } from "logitar-vue3-ui";
import { parsingUtils } from "logitar-vue3-ui";

import FormInput from "@/components/forms/FormInput.vue";

const { parseNumber } = parsingUtils;

withDefaults(
  defineProps<{
    describedBy?: string;
    disabled?: boolean | string;
    floating?: boolean | string;
    id?: string;
    label?: string;
    max?: number | string;
    min?: number | string;
    modelValue?: number;
    name?: string;
    placeholder?: string;
    plaintext?: boolean | string;
    readonly?: boolean | string;
    required?: boolean | string;
    size?: InputSize;
    status?: InputStatus;
    step?: number | string;
    type?: InputType;
  }>(),
  {
    floating: true,
    type: "number",
  },
);

const emit = defineEmits<{
  (e: "update:model-value", value: number | undefined): void;
}>();

function onModelValueUpdate(value: string): void {
  try {
    const number: number | undefined = parseNumber(value);
    emit("update:model-value", typeof number === "number" && !isNaN(number) ? number : undefined);
  } catch (_) {
    emit("update:model-value", undefined);
  }
}
</script>

<template>
  <FormInput
    :described-by="describedBy"
    :disabled="disabled"
    :floating="floating"
    :id="id"
    :label="label"
    :max="max"
    :min="min"
    :model-value="modelValue?.toString()"
    :name="name"
    :plaintext="plaintext"
    :readonly="readonly"
    :required="required"
    :size="size"
    :status="status"
    :step="step"
    :type="type"
    @update:model-value="onModelValueUpdate"
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
      <slot name="after"></slot>
    </template>
  </FormInput>
</template>
