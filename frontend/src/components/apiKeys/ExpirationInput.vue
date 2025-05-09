<script setup lang="ts">
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import DateTimeInput from "@/components/shared/DateTimeInput.vue";

const { t } = useI18n();

const props = withDefaults(
  defineProps<{
    disabled?: boolean | string;
    id?: string;
    label?: string;
    max?: Date | null;
    min?: Date | null;
    modelValue?: Date;
    required?: boolean | string;
  }>(),
  {
    id: "expiration",
    label: "apiKeys.expiresOn",
    min: () => new Date(),
  },
);

const isExpired = computed<boolean>(() => Boolean(props.modelValue && props.modelValue <= new Date()));

defineEmits<{
  (e: "update:model-value", value: Date | undefined): void;
}>();
</script>

<template>
  <DateTimeInput
    :disabled="disabled"
    :id="id"
    :label="t(label)"
    :max="max ?? undefined"
    :min="min ?? undefined"
    :model-value="modelValue"
    :required="required"
    @update:model-value="$emit('update:model-value', $event)"
  >
    <template v-if="isExpired" #append>
      <span class="input-group-text"><font-awesome-icon class="me-1" icon="fas fa-hourglass-end" />{{ t("apiKeys.expired") }}</span>
    </template>
  </DateTimeInput>
</template>
