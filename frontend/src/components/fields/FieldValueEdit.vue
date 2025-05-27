<script setup lang="ts">
import BooleanFieldValueEdit from "./BooleanFieldValueEdit.vue";
import DateTimeFieldValueEdit from "./DateTimeFieldValueEdit.vue";
import NumberFieldValueEdit from "./NumberFieldValueEdit.vue";
import RelatedContentFieldValueEdit from "./RelatedContentFieldValueEdit.vue";
import RichTextFieldValueEdit from "./RichTextFieldValueEdit.vue";
import SelectFieldValueEdit from "./SelectFieldValueEdit.vue";
import StringFieldValueEdit from "./StringFieldValueEdit.vue";
import TagsFieldValueEdit from "./TagsFieldValueEdit.vue";
import type { FieldDefinition } from "@/types/fields";
import type { Language } from "@/types/languages";

defineProps<{
  field: FieldDefinition;
  language?: Language;
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
    <RelatedContentFieldValueEdit
      v-else-if="field.fieldType.dataType === 'RelatedContent'"
      :field="field"
      :language="language"
      :model-value="modelValue"
      @update:model-value="$emit('update:model-value', $event)"
    />
    <RichTextFieldValueEdit
      v-else-if="field.fieldType.dataType === 'RichText'"
      :field="field"
      :model-value="modelValue"
      @update:model-value="$emit('update:model-value', $event)"
    />
    <SelectFieldValueEdit
      v-else-if="field.fieldType.dataType === 'Select'"
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
    <TagsFieldValueEdit
      v-else-if="field.fieldType.dataType === 'Tags'"
      :field="field"
      :model-value="modelValue"
      @update:model-value="$emit('update:model-value', $event)"
    />
  </div>
</template>
