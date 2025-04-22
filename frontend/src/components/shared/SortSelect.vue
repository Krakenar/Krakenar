<script setup lang="ts">
import { TarCheckbox, TarSelect, type SelectOption } from "logitar-vue3-ui";
import { useI18n } from "vue-i18n";

const { t } = useI18n();

withDefaults(
  defineProps<{
    descending?: boolean | string;
    id?: string;
    label?: string;
    modelValue?: string;
    options?: SelectOption[];
    placeholder?: string;
  }>(),
  {
    id: "sort",
    label: "sort.select.label",
    placeholder: "sort.select.placeholder",
  },
);

defineEmits<{
  (e: "descending", value: boolean): void;
  (e: "update:model-value", value: string): void;
}>();
</script>

<template>
  <TarSelect
    floating
    :id="id"
    :label="t(label)"
    :model-value="modelValue"
    :options="options"
    :placeholder="t(placeholder)"
    @update:model-value="$emit('update:model-value', $event)"
  >
    <template #after>
      <TarCheckbox :id="`${id}-desc`" :label="t('sort.isDescending')" :model-value="descending" @update:model-value="$emit('descending', $event)" />
    </template>
  </TarSelect>
</template>
