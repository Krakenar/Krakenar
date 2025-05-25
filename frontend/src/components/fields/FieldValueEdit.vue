<script setup lang="ts">
import BooleanFieldValueEdit from "./BooleanFieldValueEdit.vue";
import DateTimeFieldValueEdit from "./DateTimeFieldValueEdit.vue";
import NumberFieldValueEdit from "./NumberFieldValueEdit.vue";
import RichTextFieldValueEdit from "./RichTextFieldValueEdit.vue";
import StringFieldValueEdit from "./StringFieldValueEdit.vue";
import type { FieldDefinition } from "@/types/fields";

defineProps<{
  field: FieldDefinition;
  modelValue?: string;
}>();

defineEmits<{
  (e: "update:model-value", value: string): void;
}>();
</script>

<template>
  <div>
    <BooleanFieldValueEdit
      v-if="field.fieldType.dataType === 'Boolean'"
      :field="field"
      :model-value="modelValue"
      @update:model-value="$emit('update:model-value', $event)"
    />
    <DateTimeFieldValueEdit
      v-else-if="field.fieldType.dataType === 'DateTime'"
      :field="field"
      :model-value="modelValue"
      @update:model-value="$emit('update:model-value', $event)"
    />
    <NumberFieldValueEdit
      v-else-if="field.fieldType.dataType === 'Number'"
      :field="field"
      :model-value="modelValue"
      @update:model-value="$emit('update:model-value', $event)"
    />
    <RichTextFieldValueEdit
      v-else-if="field.fieldType.dataType === 'RichText'"
      :field="field"
      :model-value="modelValue"
      @update:model-value="$emit('update:model-value', $event)"
    />
    <StringFieldValueEdit
      v-else-if="field.fieldType.dataType === 'String'"
      :field="field"
      :model-value="modelValue"
      @update:model-value="$emit('update:model-value', $event)"
    />
  </div>
</template>
