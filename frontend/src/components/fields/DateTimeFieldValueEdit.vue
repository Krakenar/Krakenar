<script setup lang="ts">
import { computed } from "vue";

import DateTimeInput from "@/components/shared/DateTimeInput.vue";
import FieldValueLabel from "./FieldValueLabel.vue";
import type { FieldDefinition, FieldType } from "@/types/fields";

const props = defineProps<{
  field: FieldDefinition;
  modelValue?: string;
}>();

const fieldType = computed<FieldType>(() => props.field.fieldType);
const maxDate = computed<Date | undefined>(() => (fieldType.value.dateTime?.maximumValue ? new Date(fieldType.value.dateTime?.maximumValue) : undefined));
const minDate = computed<Date | undefined>(() => (fieldType.value.dateTime?.minimumValue ? new Date(fieldType.value.dateTime?.minimumValue) : undefined));
const dateValue = computed<Date | undefined>(() => (props.modelValue ? new Date(props.modelValue) : undefined));

const emit = defineEmits<{
  (e: "update:model-value", value: string): void;
}>();

function onModelValueUpdate(value: Date | undefined): void {
  emit("update:model-value", value?.toISOString() ?? "");
}
</script>

<template>
  <DateTimeInput
    :described-by="field.description ? `${field.id}-help` : undefined"
    :id="field.id"
    :max="maxDate"
    :min="minDate"
    :model-value="dateValue"
    :name="field.uniqueName"
    @update:model-value="onModelValueUpdate"
  >
    <template #label-override>
      <FieldValueLabel :field="field" />
    </template>
    <template v-if="field.description" #after>
      <div :id="`${field.id}-help`" class="form-text">{{ field.description }}</div>
    </template>
  </DateTimeInput>
</template>
