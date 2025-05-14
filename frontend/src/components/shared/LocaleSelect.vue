<script setup lang="ts">
import type { SelectOption } from "logitar-vue3-ui";
import { arrayUtils } from "logitar-js";
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import FormSelect from "@/components/forms/FormSelect.vue";
import locales from "@/resources/locales.json";
import type { Locale } from "@/types/i18n";

const { orderBy } = arrayUtils;
const { t } = useI18n();

withDefaults(
  defineProps<{
    id?: string;
    label?: string;
    modelValue?: string;
    placeholder?: string;
    required?: boolean | string;
  }>(),
  {
    id: "locale",
    label: "locale.label",
    placeholder: "locale.placeholder",
  },
);

const options = computed<SelectOption[]>(() =>
  orderBy(
    locales.map(({ code, nativeName }) => ({ value: code, text: nativeName })),
    "text",
  ),
);

const emit = defineEmits<{
  (e: "selected", value: Locale | undefined): void;
  (e: "update:model-value", value: string): void;
}>();

function onModelValueUpdate(value: string): void {
  emit("update:model-value", value);

  const locale: Locale | undefined = locales.find(({ code }) => code === value);
  emit("selected", locale);
}
</script>

<template>
  <FormSelect
    :id="id"
    :label="t(label)"
    :model-value="modelValue"
    :options="options"
    :placeholder="t(placeholder)"
    :required="required"
    @update:model-value="onModelValueUpdate"
  >
    <template #after>
      <slot name="after"></slot>
    </template>
  </FormSelect>
</template>
