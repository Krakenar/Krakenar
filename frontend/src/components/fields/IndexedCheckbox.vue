<script setup lang="ts">
import { TarCheckbox } from "logitar-vue3-ui";
import { useI18n } from "vue-i18n";

const { t } = useI18n();

withDefaults(
  defineProps<{
    id?: string;
    label?: string;
    modelValue?: boolean;
  }>(),
  {
    id: "indexed",
    label: "fields.definition.indexed.label",
  },
);

defineEmits<{
  (e: "update:model-value", value: boolean): void;
}>();
</script>

<template>
  <TarCheckbox
    class="mb-3"
    :described-by="`${id}-help`"
    :id="id"
    :label="t(label)"
    :model-value="modelValue"
    @update:model-value="$emit('update:model-value', $event)"
  >
    <template #after>
      <div :id="`${id}-help`" class="form-text">{{ t("fields.definition.indexed.help") }}</div>
    </template>
  </TarCheckbox>
</template>
