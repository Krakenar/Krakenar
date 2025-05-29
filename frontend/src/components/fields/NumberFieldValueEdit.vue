<script setup lang="ts">
import { computed } from "vue";
import { parsingUtils } from "logitar-js";

import FieldValueLabel from "./FieldValueLabel.vue";
import NumberInput from "@/components/shared/NumberInput.vue";
import type { FieldDefinition, FieldType } from "@/types/fields";

const { parseNumber } = parsingUtils;

const props = defineProps<{
  field: FieldDefinition;
  modelValue?: string;
}>();

const fieldType = computed<FieldType>(() => props.field.fieldType);
const numberValue = computed<number | undefined>(() => (props.modelValue ? parseNumber(props.modelValue) : undefined));

const emit = defineEmits<{
  (e: "update:model-value", value: string): void;
}>();

function onModelValueUpdate(value: number | undefined): void {
  emit("update:model-value", value?.toString() ?? "");
}
</script>

<template>
  <NumberInput
    :described-by="field.description ? `${field.id}-help` : undefined"
    :id="field.id"
    :max="fieldType.number?.maximumValue ?? undefined"
    :min="fieldType.number?.minimumValue ?? undefined"
    :model-value="numberValue"
    :name="field.uniqueName"
    :placeholder="field.placeholder ?? field.displayName ?? field.uniqueName"
    :step="fieldType.number?.step ?? undefined"
    @update:model-value="onModelValueUpdate"
  >
    <template #label-override>
      <FieldValueLabel :field="field" />
    </template>
    <template v-if="field.description" #after>
      <div :id="`${field.id}-help`" class="form-text">{{ field.description }}</div>
    </template>
  </NumberInput>
</template>
