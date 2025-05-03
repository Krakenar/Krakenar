<script setup lang="ts">
import { useI18n } from "vue-i18n";

import NumberInput from "@/components/shared/NumberInput.vue";

const { t } = useI18n();

withDefaults(
  defineProps<{
    id?: string;
    label?: string;
    min?: number | string;
    modelValue?: number;
  }>(),
  {
    id: "lifetime",
    label: "tokens.lifetime.label",
    min: 0,
  },
);

defineEmits<{
  (e: "update:model-value", value: number | undefined): void;
}>();
</script>

<template>
  <NumberInput
    :id="id"
    :label="t(label)"
    :min="min"
    :model-value="modelValue"
    :placeholder="t(label)"
    @update:model-value="$emit('update:model-value', $event)"
  >
    <template #append>
      <span class="input-group-text">{{ t("tokens.lifetime.unit", modelValue ?? 0) }}</span>
    </template>
  </NumberInput>
</template>
