<script setup lang="ts">
import { computed } from "vue";

import ContentFormMultiSelect from "@/components/contents/ContentFormMultiSelect.vue";
import ContentFormSelect from "@/components/contents/ContentFormSelect.vue";
import FieldValueLabel from "./FieldValueLabel.vue";
import type { FieldDefinition, FieldType } from "@/types/fields";
import type { Language } from "@/types/languages";

const props = defineProps<{
  field: FieldDefinition;
  language?: Language;
  modelValue?: string;
}>();

const fieldType = computed<FieldType>(() => props.field.fieldType);
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
    <ContentFormMultiSelect
      v-if="fieldType.relatedContent?.isMultiple"
      :content-type="fieldType.relatedContent?.contentTypeId"
      :described-by="field.description ? `${field.id}-help` : undefined"
      :id="field.id"
      :language="language?.id"
      :model-value="values"
      :name="field.uniqueName"
      :placeholder="field.placeholder ?? field.displayName ?? field.uniqueName"
      @update:model-value="onOptionsSelected"
    >
      <template #label-override>
        <FieldValueLabel :field="field" />
      </template>
      <template v-if="field.description" #after>
        <div :id="`${field.id}-help`" class="form-text">{{ field.description }}</div>
      </template>
    </ContentFormMultiSelect>
    <ContentFormSelect
      v-else
      :content-type="fieldType.relatedContent?.contentTypeId"
      :described-by="field.description ? `${field.id}-help` : undefined"
      :id="field.id"
      :language="language?.id"
      :model-value="value"
      :name="field.uniqueName"
      :placeholder="field.placeholder ?? field.displayName ?? field.uniqueName"
      @update:model-value="onOptionSelected"
    >
      <template #label-override>
        <FieldValueLabel :field="field" />
      </template>
      <template v-if="field.description" #after>
        <div :id="`${field.id}-help`" class="form-text">{{ field.description }}</div>
      </template>
    </ContentFormSelect>
  </div>
</template>
