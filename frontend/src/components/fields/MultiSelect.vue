<script setup lang="ts">
import type { SelectOption } from "logitar-vue3-ui";
import { computed } from "vue";
import { nanoid } from "nanoid";

import FormTextarea from "@/components/forms/FormTextarea.vue";

const props = withDefaults(
  defineProps<{
    describedBy?: string;
    id?: string;
    label?: string;
    modelValue?: string[];
    name?: string;
    options?: SelectOption[];
    placeholder?: string;
    required?: boolean | string;
  }>(),
  {
    id: () => nanoid(),
    modelValue: () => [],
    options: () => [],
  },
);

const formattedValue = computed<string>(() => props.modelValue.map((value) => indexedOptions.value.get(value) ?? value).join("\n"));
const indexedOptions = computed<Map<string, string>>(() => {
  const index = new Map<string, string>();
  props.options.forEach((option) => {
    const text: string | undefined = option.label ?? option.text;
    const value: string | undefined = option.value ?? option.text;
    if (text && value) {
      index.set(value, text);
    }
  });
  return index;
});
const rows = computed<number>(() => 1 + props.modelValue.length);

const emit = defineEmits<{
  (e: "update:model-value", values: string[]): void;
}>();

function isSelected(option: SelectOption): boolean {
  const value: string | undefined = option.value ?? option.text;
  return Boolean(value && props.modelValue.includes(value));
}
function toggle(option: SelectOption): void {
  const value: string | undefined = option.value ?? option.text;
  if (value) {
    const selected: string[] = [...props.modelValue];
    const index: number = selected.findIndex((val) => val === value);
    if (index < 0) {
      selected.push(value);
    } else {
      selected.splice(index, 1);
    }
    emit("update:model-value", selected);
  }
}
</script>

<template>
  <div class="dropdown">
    <FormTextarea
      aria-expanded="false"
      data-bs-auto-close="outside"
      data-bs-toggle="dropdown"
      :described-by="describedBy"
      floating
      :id="id"
      :label="label"
      :model-value="formattedValue"
      :name="name"
      :placeholder="label"
      readonly
      :required="required"
      :rows="rows"
    >
      <template #label-override>
        <slot name="label-override"></slot>
      </template>
      <template #after>
        <slot name="after"></slot>
      </template>
    </FormTextarea>
    <ul class="dropdown-menu">
      <li v-if="placeholder" class="disabled dropdown-item">{{ placeholder }}</li>
      <li
        v-for="option in options"
        :key="option.value"
        :class="{ active: isSelected(option), disabled: option.disabled, 'dropdown-item': true }"
        @click.prevent="toggle(option)"
      >
        <font-awesome-icon v-if="isSelected(option)" icon="fas fa-check" /> {{ option.text }}
      </li>
    </ul>
  </div>
</template>

<style scoped>
.dropdown-item {
  cursor: pointer;
}
</style>
