<script setup lang="ts">
import { useI18n } from "vue-i18n";

import DateTimeInput from "@/components/shared/DateTimeInput.vue";
import { computed } from "vue";

const { t } = useI18n();

const props = withDefaults(
  defineProps<{
    id?: string;
    label?: string;
    max?: Date | null;
    min?: Date | null;
    modelValue?: Date;
  }>(),
  {
    id: "birthdate",
    label: "users.birthdate",
    max: () => new Date(),
  },
);

const inputMin = computed<Date | undefined>(() => {
  if (props.min) {
    return props.min;
  } else if (props.min !== null && props.max) {
    const min: Date = new Date(props.max);
    min.setFullYear(min.getFullYear() - 120);
    return min;
  }
  return undefined;
});

defineEmits<{
  (e: "update:model-value", value: Date | undefined): void;
}>();
</script>

<template>
  <DateTimeInput
    :id="id"
    :label="t(label)"
    :max="max ?? undefined"
    :min="inputMin"
    :model-value="modelValue"
    @update:model-value="$emit('update:model-value', $event)"
  />
</template>
