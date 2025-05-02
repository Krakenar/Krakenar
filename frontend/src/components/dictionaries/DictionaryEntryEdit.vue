<script setup lang="ts">
import { TarButton } from "logitar-vue3-ui";
import { useI18n } from "vue-i18n";

import DictionaryEntryKey from "./DictionaryEntryKey.vue";
import DictionaryEntryValue from "./DictionaryEntryValue.vue";
import type { DictionaryEntry } from "@/types/dictionaries";

const { t } = useI18n();

const props = withDefaults(
  defineProps<{
    id?: string;
    modelValue: DictionaryEntry;
  }>(),
  {
    id: "edit-entry",
  },
);

const emit = defineEmits<{
  (e: "removed"): void;
  (e: "restored"): void;
  (e: "update:model-value", value: DictionaryEntry): void;
}>();

function onKeyChange(key: string): void {
  const entry: DictionaryEntry = { ...props.modelValue, key };
  emit("update:model-value", entry);
}
function onValueChange(value: string): void {
  const entry: DictionaryEntry = { ...props.modelValue, value };
  emit("update:model-value", entry);
}
</script>

<template>
  <div class="row">
    <DictionaryEntryKey class="col" :id="`${id}-key`" :model-value="modelValue.key" required @update:model-value="onKeyChange">
      <template #append>
        <TarButton v-if="modelValue.isRemoved" icon="fas fa-ban" :text="t('actions.restore')" variant="warning" @click="$emit('restored')" />
        <TarButton v-else icon="fas fa-times" :text="t('actions.remove')" variant="danger" @click="$emit('removed')" />
      </template>
    </DictionaryEntryKey>
    <DictionaryEntryValue class="col" :id="`${id}-value`" :model-value="modelValue.value" required @update:model-value="onValueChange" />
  </div>
</template>
