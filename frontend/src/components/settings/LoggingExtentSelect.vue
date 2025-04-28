<script setup lang="ts">
import type { SelectOption } from "logitar-vue3-ui";
import { arrayUtils } from "logitar-js";
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import FormSelect from "@/components/forms/FormSelect.vue";

const { orderBy } = arrayUtils;
const { rt, t, tm } = useI18n();

withDefaults(
  defineProps<{
    id?: string;
    label?: string;
    modelValue: string;
    placeholder?: string;
    required?: boolean | string;
  }>(),
  {
    id: "extent",
    label: "settings.logging.extent.label",
    placeholder: "settings.logging.extent.placeholder",
    required: true,
  },
);

const options = computed<SelectOption[]>(() =>
  orderBy(
    Object.entries(tm(rt("settings.logging.extent.options"))).map(([value, text]) => ({ text, value }) as SelectOption),
    "text",
  ),
);

defineEmits<{
  (e: "update:model-value", value: string): void;
}>();
</script>

<template>
  <FormSelect
    :described-by="`${id}-help`"
    :id="id"
    :label="t(label)"
    :model-value="modelValue"
    :options="options"
    :placeholder="t(placeholder)"
    :required="required"
    @update:model-value="$emit('update:model-value', $event)"
  >
    <template #after>
      <div :id="`${id}-help`" class="form-text">{{ t(`settings.logging.extent.help.${modelValue}`) }}</div>
    </template>
  </FormSelect>
</template>
