<script setup lang="ts">
import UniqueNameInput from "@/components/shared/UniqueNameInput.vue";

withDefaults(
  defineProps<{
    id?: string;
    label?: string;
    modelValue?: string;
    required?: boolean | string;
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
  <UniqueNameInput :id="id" :label="label" :model-value="modelValue" :required="required" @update:model-value="$emit('update:model-value', $event)" />
</template>
