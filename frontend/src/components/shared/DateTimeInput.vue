<script setup lang="ts">
import type { InputSize, InputStatus, InputType } from "logitar-vue3-ui";
import { computed } from "vue";
import { dateUtils } from "logitar-js";

import FormInput from "@/components/forms/FormInput.vue";

const { toDateTimeLocal } = dateUtils;

const props = withDefaults(
  defineProps<{
    describedBy?: string;
    disabled?: boolean | string;
    floating?: boolean | string;
    id?: string;
    label?: string;
    max?: Date;
    min?: Date;
    modelValue?: Date;
    name?: string;
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
    type: "datetime-local",
  },
);

const inputMax = computed<string | undefined>(() => (props.max ? toDateTimeLocal(props.max) : undefined));
const inputMin = computed<string | undefined>(() => (props.min ? toDateTimeLocal(props.min) : undefined));
const inputValue = computed<string | undefined>(() => (props.modelValue ? toDateTimeLocal(props.modelValue) : undefined));

const emit = defineEmits<{
  (e: "update:model-value", value: Date | undefined): void;
}>();

function onModelValueUpdate(value: string): void {
  try {
    const date: Date = new Date(value);
    emit("update:model-value", isNaN(date.getTime()) ? undefined : date);
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
    :max="inputMax"
    :min="inputMin"
    :model-value="inputValue"
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
