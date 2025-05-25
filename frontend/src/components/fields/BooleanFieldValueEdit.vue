<script setup lang="ts">
import { TarCheckbox } from "logitar-vue3-ui";

import type { FieldDefinition } from "@/types/fields";

defineProps<{
  field: FieldDefinition;
  modelValue?: string;
}>();

const emit = defineEmits<{
  (e: "update:model-value", value: string): void;
}>();

function onModelValueUpdate(value: boolean): void {
  emit("update:model-value", value.toString());
}
</script>

<template>
  <TarCheckbox
    class="mb-3"
    :described-by="field.description ? `${field.id}-help` : undefined"
    :id="field.id"
    :label="field.displayName ?? field.uniqueName"
    :model-value="modelValue"
    :name="field.uniqueName"
    :required="field.isRequired"
    @update:model-value="onModelValueUpdate"
  >
    <template v-if="field.description" #after>
      <div :id="`${field.id}-help`" class="form-text">{{ field.description }}</div>
    </template>
  </TarCheckbox>
</template>
