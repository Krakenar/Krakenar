<script setup lang="ts">
import UniqueNameInput from "../shared/UniqueNameInput.vue";

withDefaults(
  defineProps<{
    id?: string;
    label?: string;
    modelValue?: string;
  }>(),
  {
    id: "username",
    label: "users.username",
  },
);

defineEmits<{
  (e: "update:model-value", value: string): void;
}>();
</script>

<template>
  <UniqueNameInput :id="id" :label="label" :model-value="modelValue" @update:model-value="$emit('update:model-value', $event)" />
</template>
