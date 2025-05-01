<script setup lang="ts">
import { TarButton } from "logitar-vue3-ui";
import { useI18n } from "vue-i18n";

import CustomAttributeKey from "./CustomAttributeKey.vue";
import CustomAttributeValue from "./CustomAttributeValue.vue";
import type { ICustomAttributeState } from "@/types/custom";

const { t } = useI18n();

const props = withDefaults(
  defineProps<{
    id?: string;
    modelValue: ICustomAttributeState;
  }>(),
  {
    id: "edit-attribute",
  },
);

const emit = defineEmits<{
  (e: "removed"): void;
  (e: "restored"): void;
  (e: "update:model-value", value: ICustomAttributeState): void;
}>();

function onKeyChange(key: string): void {
  const state: ICustomAttributeState = props.modelValue;
  state.setCurrentKey(key);
  emit("update:model-value", state);
}
function onValueChange(value: string): void {
  const state: ICustomAttributeState = props.modelValue;
  state.setCurrentValue(value);
  emit("update:model-value", state);
}
</script>

<template>
  <div class="row">
    <CustomAttributeKey class="col" :id="`${id}-key`" :model-value="modelValue.getCurrentKey()" required @update:model-value="onKeyChange">
      <template #append>
        <TarButton v-if="modelValue.status === 'removed'" icon="fas fa-ban" :text="t('actions.restore')" variant="warning" @click="$emit('restored')" />
        <TarButton v-else icon="fas fa-times" :text="t('actions.remove')" variant="danger" @click="$emit('removed')" />
      </template>
    </CustomAttributeKey>
    <CustomAttributeValue class="col" :id="`${id}-value`" :model-value="modelValue.getCurrentValue()" required @update:model-value="onValueChange" />
  </div>
</template>
