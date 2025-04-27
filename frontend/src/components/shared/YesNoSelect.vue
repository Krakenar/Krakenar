<script setup lang="ts">
import type { SelectOption } from "logitar-vue3-ui";
import { arrayUtils, parsingUtils } from "logitar-js";
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import AppSelect from "./AppSelect.vue";

const { orderBy } = arrayUtils;
const { parseBoolean } = parsingUtils;
const { t } = useI18n();

withDefaults(
  defineProps<{
    id?: string;
    label?: string;
    modelValue?: boolean | string;
    placeholder?: string;
  }>(),
  {
    placeholder: "placeholder",
  },
);

const options = computed<SelectOption[]>(() =>
  orderBy(
    [
      {
        text: t("no"),
        value: false.toString(),
      },
      {
        text: t("yes"),
        value: true.toString(),
      },
    ],
    "text",
  ),
);

defineEmits<{
  (e: "update:model-value", value: boolean | undefined): void;
}>();
</script>

<template>
  <AppSelect
    :id="id"
    :label="label ? t(label) : undefined"
    :model-value="modelValue?.toString()"
    :options="options"
    :placeholder="placeholder ? t(placeholder) : undefined"
    @update:model-value="$emit('update:model-value', parseBoolean($event))"
  />
</template>
