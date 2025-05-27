<script setup lang="ts">
import type { SelectOption } from "logitar-vue3-ui";
import { computed } from "vue";

import FormSelect from "@/components/forms/FormSelect.vue";
import MultiSelect from "./MultiSelect.vue";
import type { FieldDefinition, FieldType } from "@/types/fields";

const props = defineProps<{
  field: FieldDefinition;
  modelValue?: string;
}>();

const fieldType = computed<FieldType>(() => props.field.fieldType);
const options = computed<SelectOption[]>(
  () =>
    fieldType.value.select?.options.map(({ text, value, label, isDisabled }) => ({
      text,
      value: value ?? undefined,
      label: label ?? undefined,
      disabled: isDisabled,
    })) ?? [],
);
const value = computed<string>(() => (values.value.length ? values.value[0] : ""));
const values = computed<string[]>(() => {
  try {
    const values: string[] = JSON.parse(props.modelValue ?? "[]");
    return values;
  } catch {
    return [];
  }
});

const emit = defineEmits<{
  (e: "update:model-value", value: string): void;
}>();

function onOptionSelected(option: string): void {
  emit("update:model-value", option ? JSON.stringify([option]) : "");
}
function onOptionsSelected(options: string[]): void {
  emit("update:model-value", options.length ? JSON.stringify(options) : "");
}
</script>

<template>
  <div>
    <MultiSelect
      v-if="fieldType.select?.isMultiple"
      :described-by="field.description ? `${field.id}-help` : undefined"
      :id="field.id"
      :label="field.displayName ?? field.uniqueName"
      :model-value="values"
      :name="field.uniqueName"
      :options="options"
      :placeholder="field.placeholder ?? field.displayName ?? field.uniqueName"
      @update:model-value="onOptionsSelected"
    >
      <template v-if="field.description" #after>
        <div :id="`${field.id}-help`" class="form-text">{{ field.description }}</div>
      </template>
    </MultiSelect>
    <FormSelect
      v-else
      :described-by="field.description ? `${field.id}-help` : undefined"
      :id="field.id"
      :label="field.displayName ?? field.uniqueName"
      :model-value="value"
      :name="field.uniqueName"
      :options="options"
      :placeholder="field.placeholder ?? field.displayName ?? field.uniqueName"
      @update:model-value="onOptionSelected"
    >
      <template v-if="field.description" #after>
        <div :id="`${field.id}-help`" class="form-text">{{ field.description }}</div>
      </template>
    </FormSelect>
  </div>
</template>
