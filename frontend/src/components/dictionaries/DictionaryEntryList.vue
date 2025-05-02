<script setup lang="ts">
import { TarButton } from "logitar-vue3-ui";
import { useI18n } from "vue-i18n";

import DictionaryEntryEdit from "./DictionaryEntryEdit.vue";
import type { DictionaryEntry } from "@/types/dictionaries";

const { t } = useI18n();

const props = defineProps<{
  modelValue: DictionaryEntry[];
}>();

const emit = defineEmits<{
  (e: "update:model-value", value: DictionaryEntry[]): void;
}>();

function onAdd(): void {
  const entry: DictionaryEntry = { key: "", value: "", isAdded: true };
  const entries: DictionaryEntry[] = [...props.modelValue, entry];
  emit("update:model-value", entries);
}
function onRemove(index: number): void {
  const entries: DictionaryEntry[] = [...props.modelValue];
  let entry: DictionaryEntry | undefined = entries[index];
  if (entry) {
    if (entry.isAdded) {
      entries.splice(index, 1);
    } else {
      entry = { ...entry, isRemoved: true };
      entries.splice(index, 1, entry);
    }
    emit("update:model-value", entries);
  }
}
function onRestore(index: number): void {
  const entries: DictionaryEntry[] = [...props.modelValue];
  let entry: DictionaryEntry | undefined = entries[index];
  if (entry) {
    entry = { ...entry, isRemoved: false };
    entries.splice(index, 1, entry);
    emit("update:model-value", entries);
  }
}
function onUpdate(index: number, entry: DictionaryEntry): void {
  const entries: DictionaryEntry[] = [...props.modelValue];
  entries.splice(index, 1, entry);
  emit("update:model-value", entries);
}
</script>

<template>
  <div>
    <h5>{{ t("dictionaries.entries.title") }}</h5>
    <div class="mb-3">
      <TarButton icon="fas fa-plus" :text="t('actions.add')" variant="success" @click="onAdd" />
    </div>
    <template v-if="modelValue.length > 0">
      <DictionaryEntryEdit
        v-for="(entry, index) in modelValue"
        :key="index"
        :id="`edit-entry-${index}`"
        :model-value="entry"
        @removed="onRemove(index)"
        @restored="onRestore(index)"
        @update:model-value="onUpdate(index, $event)"
      />
    </template>
    <p v-else>{{ t("dictionaries.entries.empty") }}</p>
  </div>
</template>
