<script setup lang="ts">
import { nanoid } from "nanoid";

import AppTag from "./AppTag.vue";
import AppNewTag from "./AppNewTag.vue";

const props = withDefaults(
  defineProps<{
    describedBy?: string;
    id?: string;
    label?: string;
    modelValue?: string[];
  }>(),
  {
    id: () => nanoid(),
    modelValue: () => [],
  },
);

const emit = defineEmits<{
  (e: "update:model-value", value: string[]): void;
}>();

function onAdd(tag: string): void {
  const tags: string[] = [...props.modelValue];
  tags.push(tag);
  emit("update:model-value", tags);
}
function onRemove(index: number): void {
  const tags: string[] = [...props.modelValue];
  tags.splice(index, 1);
  emit("update:model-value", tags);
}
function onUpdate(index: number, tag: string): void {
  const tags: string[] = [...props.modelValue];
  tags.splice(index, 1, tag);
  emit("update:model-value", tags);
}
</script>

<template>
  <div class="mb-3">
    <div class="form-control" :aria-describedby="describedBy">
      <slot name="label-override">
        <label v-if="label" :for="id" class="mb-2">{{ label }}</label>
      </slot>
      <br />
      <AppTag
        v-for="(tag, index) in modelValue"
        :key="index"
        class="mb-2 me-2 tag"
        :value="tag"
        @removed="onRemove(index)"
        @updated="onUpdate(index, $event)"
      />
      <AppNewTag class="tag" :id="id" @added="onAdd" />
    </div>
    <slot name="after"></slot>
  </div>
</template>

<style scoped>
.tag {
  display: inline-block;
  max-width: 320px;
}
</style>
