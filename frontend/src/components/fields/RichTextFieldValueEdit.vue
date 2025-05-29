<script setup lang="ts">
import { computed } from "vue";

import FieldValueLabel from "./FieldValueLabel.vue";
import FormTextarea from "@/components/forms/FormTextarea.vue";
import type { FieldDefinition, FieldType } from "@/types/fields";

const props = withDefaults(
  defineProps<{
    field: FieldDefinition;
    modelValue?: string;
    rows?: number | string;
  }>(),
  {
    rows: 15,
  },
);

const fieldType = computed<FieldType>(() => props.field.fieldType);

defineEmits<{
  (e: "update:model-value", value: string): void;
}>();
</script>

<template>
  <FormTextarea
    :described-by="field.description ? `${field.id}-help` : undefined"
    :id="field.id"
    :max="fieldType.string?.maximumLength ?? undefined"
    :min="fieldType.string?.minimumLength ?? undefined"
    :model-value="modelValue"
    :name="field.uniqueName"
    :placeholder="field.placeholder ?? field.displayName ?? field.uniqueName"
    :rows="rows"
    @update:model-value="$emit('update:model-value', $event)"
  >
    <template #label-override>
      <FieldValueLabel :field="field" />
    </template>
    <template v-if="field.description" #after>
      <div :id="`${field.id}-help`" class="form-text">{{ field.description }}</div>
    </template>
  </FormTextarea>
</template>
