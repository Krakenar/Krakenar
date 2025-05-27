<script setup lang="ts">
import { computed } from "vue";

import AppTags from "@/components/tags/AppTags.vue";
import type { FieldDefinition } from "@/types/fields";

const props = defineProps<{
  field: FieldDefinition;
  modelValue?: string;
}>();

const tags = computed<string[]>(() => {
  try {
    return JSON.parse(props.modelValue ?? "[]");
  } catch {
    return [];
  }
});

const emit = defineEmits<{
  (e: "update:model-value", value: string): void;
}>();

function onModelValueUpdate(tags: string[]): void {
  emit("update:model-value", tags.length > 0 ? JSON.stringify(tags) : "");
}
</script>

<template>
  <AppTags
    :described-by="field.description ? `${field.id}-help` : undefined"
    :id="field.id"
    :label="field.displayName ?? field.uniqueName"
    :model-value="tags"
    @update:model-value="onModelValueUpdate"
  >
    <template v-if="field.description" #after>
      <div :id="`${field.id}-help`" class="form-text">{{ field.description }}</div>
    </template>
  </AppTags>
</template>
