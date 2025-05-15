<script setup lang="ts">
import { TarCheckbox } from "logitar-vue3-ui";
import { useI18n } from "vue-i18n";

const { t } = useI18n();

withDefaults(
  defineProps<{
    help?: string;
    id?: string;
    label?: string;
    modelValue?: boolean | string;
  }>(),
  {
    help: "contents.type.invariant.help",
    id: "invariant",
    label: "contents.type.invariant.label",
  },
);

defineEmits<{
  (e: "update:model-value", value: boolean): void;
}>();
</script>

<template>
  <div class="mb-3">
    <TarCheckbox :described-by="`${id}-help`" :id="id" :label="t(label)" :model-value="modelValue" @update:model-value="$emit('update:model-value', $event)">
      <template #after>
        <div :id="`${id}-help`" class="form-text">{{ t(help) }}</div>
      </template>
    </TarCheckbox>
  </div>
</template>
