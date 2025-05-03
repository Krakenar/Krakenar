<script setup lang="ts">
import { TarButton } from "logitar-vue3-ui";
import { useI18n } from "vue-i18n";

import ClaimName from "./ClaimName.vue";
import ClaimType from "./ClaimType.vue";
import ClaimValue from "./ClaimValue.vue";
import type { Claim } from "@/types/tokens";

const { t } = useI18n();

const props = withDefaults(
  defineProps<{
    id?: string;
    modelValue: Claim;
  }>(),
  {
    id: "edit-claim",
  },
);

const emit = defineEmits<{
  (e: "removed"): void;
  (e: "update:model-value", claim: Claim): void;
}>();

function onNameUpdate(name: string): void {
  const claim: Claim = { ...props.modelValue, name };
  emit("update:model-value", claim);
}
function onTypeUpdate(type: string): void {
  const claim: Claim = { ...props.modelValue, type };
  emit("update:model-value", claim);
}
function onValueUpdate(value: string): void {
  const claim: Claim = { ...props.modelValue, value };
  emit("update:model-value", claim);
}
</script>

<template>
  <div class="row">
    <ClaimName class="col" :id="`${id}-name`" :model-value="modelValue.name" required @update:model-value="onNameUpdate">
      <template #append>
        <TarButton icon="fas fa-times" :text="t('actions.remove')" variant="danger" @click="$emit('removed')" />
      </template>
    </ClaimName>
    <ClaimValue class="col" :id="`${id}-value`" :model-value="modelValue.value" required @update:model-value="onValueUpdate" />
    <ClaimType class="col" :id="`${id}-type`" :model-value="modelValue.type ?? ''" @update:model-value="onTypeUpdate" />
  </div>
</template>
