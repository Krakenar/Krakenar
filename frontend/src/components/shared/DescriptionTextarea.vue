<script setup lang="ts">
import { useI18n } from "vue-i18n";

import FormTextarea from "@/components/forms/FormTextarea.vue";

const { t } = useI18n();

withDefaults(
  defineProps<{
    help?: string;
    id?: string;
    label?: string;
    modelValue?: string;
    rows?: number | string;
  }>(),
  {
    id: "description",
    label: "description",
    rows: 15,
  },
);

defineEmits<{
  (e: "update:model-value", value: string): void;
}>();
</script>

<template>
  <FormTextarea
    :described-by="help ? `${id}-help` : undefined"
    :id="id"
    :label="t(label)"
    :model-value="modelValue"
    :rows="rows"
    @update:model-value="$emit('update:model-value', $event)"
  >
    <template v-if="help" #after>
      <div :id="`${id}-help`" class="form-text">{{ t(help) }}</div>
    </template>
  </FormTextarea>
</template>
