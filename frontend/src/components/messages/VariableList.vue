<script setup lang="ts">
import { TarButton } from "logitar-vue3-ui";
import { useI18n } from "vue-i18n";

import VariableEdit from "./VariableEdit.vue";
import type { Variable } from "@/types/messages";

const { t } = useI18n();

const props = defineProps<{
  modelValue: Variable[];
}>();

const emit = defineEmits<{
  (e: "update:model-value", variable: Variable[]): void;
}>();

function onAdd(): void {
  const variable: Variable = { key: "", value: "" };
  const variables: Variable[] = [...props.modelValue, variable];
  emit("update:model-value", variables);
}
function onRemove(index: number): void {
  const variables: Variable[] = [...props.modelValue];
  const variable: Variable | undefined = variables[index];
  if (variable) {
    variables.splice(index, 1);
    emit("update:model-value", variables);
  }
}
function onUpdate(index: number, variable: Variable): void {
  const variables: Variable[] = [...props.modelValue];
  variables.splice(index, 1, variable);
  emit("update:model-value", variables);
}
</script>

<template>
  <div>
    <h5>{{ t("messages.variables.title") }}</h5>
    <div class="mb-3">
      <TarButton icon="fas fa-plus" :text="t('actions.add')" variant="success" @click="onAdd" />
    </div>
    <VariableEdit
      v-for="(variable, index) in modelValue"
      :key="index"
      :id="`variable-${index}`"
      :model-value="variable"
      @removed="onRemove(index)"
      @update:model-value="onUpdate(index, $event)"
    />
  </div>
</template>
