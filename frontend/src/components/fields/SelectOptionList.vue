<script setup lang="ts">
import { TarButton } from "logitar-vue3-ui";
import { useI18n } from "vue-i18n";

import SelectOptionEdit from "./SelectOptionEdit.vue";
import type { SelectOption } from "@/types/fields";

const { t } = useI18n();

const props = defineProps<{
  modelValue: SelectOption[];
}>();

const emit = defineEmits<{
  (e: "update:model-value", value: SelectOption[]): void;
}>();

function onAdd(): void {
  const option: SelectOption = { text: "", value: "", label: "", isDisabled: false };
  const options: SelectOption[] = [...props.modelValue, option];
  emit("update:model-value", options);
}
function onMove(index: number, direction: -1 | 1): void {
  // NOTE(fpion): -1 moves up the option, +1 moves down the option.
  const options: SelectOption[] = [...props.modelValue];
  const option: SelectOption | undefined = options[index];
  if (option) {
    options.splice(index, 1);
    options.splice(index + direction, 0, option);
    emit("update:model-value", options);
  }
}
function onRemove(index: number): void {
  const options: SelectOption[] = [...props.modelValue];
  const option: SelectOption | undefined = options[index];
  if (option) {
    options.splice(index, 1);
    emit("update:model-value", options);
  }
}
function onUpdate(index: number, option: SelectOption): void {
  const options: SelectOption[] = [...props.modelValue];
  options.splice(index, 1, option);
  emit("update:model-value", options);
}
</script>

<template>
  <div>
    <h5>{{ t("fields.type.select.option.title") }}</h5>
    <div class="mb-3">
      <TarButton icon="fas fa-plus" :text="t('actions.add')" variant="success" @click="onAdd" />
    </div>
    <SelectOptionEdit
      v-for="(option, index) in modelValue"
      :key="index"
      :first="index === 0"
      :id="`option-${index}`"
      :last="index === modelValue.length - 1"
      :model-value="option"
      @down="onMove(index, +1)"
      @removed="onRemove(index)"
      @up="onMove(index, -1)"
      @update:model-value="onUpdate(index, $event)"
    />
  </div>
</template>
