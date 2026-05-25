<script setup lang="ts">
import type { InputType } from "logitar-vue3-ui";
import { useI18n } from "vue-i18n";

const { t } = useI18n();

import NumberInput from "@/components/shared/NumberInput.vue";

withDefaults(
  defineProps<{
    id?: string;
    label?: string;
    max?: number | string;
    min?: number | string;
    modelValue?: number;
    required?: boolean | string;
    type?: InputType;
  }>(),
  {
    id: "port",
    label: "senders.smtpProvider.port.label",
    max: 65535,
    min: 0,
  },
);

defineEmits<{
  (e: "update:model-value", value: number | undefined): void;
}>();
</script>

<template>
  <NumberInput
    :described-by="`${id}-help`"
    :id="id"
    :label="t(label)"
    :max="max"
    :min="min"
    :model-value="modelValue"
    :required="required"
    @update:model-value="$emit('update:model-value', $event)"
  >
    <template #after>
      <div :id="`${id}-help`" class="form-text">{{ t("senders.smtpProvider.port.help") }}</div>
    </template>
  </NumberInput>
</template>
