<script setup lang="ts">
import { TarButton } from "logitar-vue3-ui";
import { useI18n } from "vue-i18n";

import VariableKey from "./VariableKey.vue";
import VariableValue from "./VariableValue.vue";
import type { Variable } from "@/types/messages";

const { t } = useI18n();

const props = withDefaults(
  defineProps<{
    id?: string;
    modelValue: Variable;
  }>(),
  {
    id: "edit-variable",
  },
);

const emit = defineEmits<{
  (e: "removed"): void;
  (e: "update:model-value", variable: Variable): void;
}>();

function onKeyUpdate(key: string): void {
  const variable: Variable = { ...props.modelValue, key };
  emit("update:model-value", variable);
}
function onValueUpdate(value: string): void {
  const variable: Variable = { ...props.modelValue, value };
  emit("update:model-value", variable);
}
</script>

<template>
  <div class="row">
    <VariableKey class="col" :id="`${id}-key`" :model-value="modelValue.key" required @update:model-value="onKeyUpdate">
      <template #append>
        <TarButton icon="fas fa-times" :text="t('actions.remove')" variant="danger" @click="$emit('removed')" />
      </template>
    </VariableKey>
    <VariableValue class="col" :id="`${id}-value`" :model-value="modelValue.value" required @update:model-value="onValueUpdate" />
  </div>
</template>
