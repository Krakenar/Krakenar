<script setup lang="ts">
import type { ValidationResult } from "logitar-validation";
import { computed, provide, ref } from "vue";

import { bindFieldKey, unbindFieldKey, type FieldOptions } from "@/types/forms";

const props = defineProps<{
  submit: () => void;
}>();

const fields = ref<Map<string, FieldOptions>>(new Map());
const isLoading = ref<boolean>(false);
const validationResults = ref<Map<string, ValidationResult>>(new Map());

const isValid = computed<boolean>(() => Object.values([...validationResults.value.values()]).filter((result) => !result.isValid).length === 0);

const emit = defineEmits<{
  (e: "loading", value: boolean): void;
}>();

function onReset(): void {
  Object.values([...fields.value.values()]).forEach((field) => field.reset());
}
function onSubmit(): void {
  if (!isLoading.value) {
    try {
      isLoading.value = true;
      emit("loading", isLoading.value);
      validate();
      if (isValid.value) {
        props.submit();
        reinitialize();
      }
    } finally {
      isLoading.value = false;
      emit("loading", isLoading.value);
    }
  }
}
function reinitialize(): void {
  Object.values([...fields.value.values()]).forEach((field) => field.reinitialize());
}
function validate(): Map<string, ValidationResult> {
  Object.entries([...fields.value.values()]).forEach(([id, field]) => {
    const result: ValidationResult = field.validate();
    validationResults.value.set(id, result);
  });
  return validationResults.value;
}
defineExpose({ reinitialize, reset: onReset, submit: onSubmit });

function bindField(id: string, options: FieldOptions): void {
  fields.value.set(id, options);
}
provide(bindFieldKey, bindField);

function unbindField(id: string): void {
  fields.value.delete(id);
  validationResults.value.delete(id);
}
provide(unbindFieldKey, unbindField);
</script>

<template>
  <form @reset.prevent="onReset" @submit.prevent="onSubmit">
    <slot></slot>
  </form>
</template>
