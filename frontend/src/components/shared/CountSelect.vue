<script setup lang="ts">
import { TarSelect, type SelectOption } from "logitar-vue3-ui";
import { parsingUtils } from "logitar-js";
import { ref } from "vue";
import { useI18n } from "vue-i18n";

const { parseNumber } = parsingUtils;
const { t } = useI18n();

withDefaults(
  defineProps<{
    id?: string;
    label?: string;
    modelValue?: number;
  }>(),
  {
    id: "count",
    label: "count",
  },
);

const options = ref<SelectOption[]>([{ text: "10" }, { text: "25" }, { text: "50" }, { text: "100" }]);

defineEmits<{
  (e: "update:model-value", value?: number): void;
}>();
</script>

<template>
  <TarSelect
    floating
    :id="id"
    :label="t(label)"
    :model-value="modelValue?.toString()"
    :options="options"
    @update:model-value="$emit('update:model-value', parseNumber($event))"
  />
</template>
