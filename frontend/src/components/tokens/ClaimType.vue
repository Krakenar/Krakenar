<script setup lang="ts">
import type { SelectOption } from "logitar-vue3-ui";
import { arrayUtils } from "logitar-js";
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import FormSelect from "@/components/forms/FormSelect.vue";
import claimTypes from "@/resources/claimTypes.json";

const { orderBy } = arrayUtils;
const { t } = useI18n();

withDefaults(
  defineProps<{
    id?: string;
    label?: string;
    modelValue?: string;
    placeholder?: string;
  }>(),
  {
    id: "type",
    label: "tokens.claims.type.label",
    placeholder: "tokens.claims.type.placeholder",
  },
);

const options = computed<SelectOption[]>(() =>
  orderBy(
    claimTypes.map(({ id, name }) => ({ text: name, value: id })),
    "text",
  ),
);

// const types = computed<SelectOption[]>(() =>
//   orderBy(
//     claimTypes.map(({ id, name }) => ({ text: name, value: id })),
//     "text",
//   ),
// );

defineEmits<{
  (e: "update:model-value", value: string): void;
}>();
</script>

<template>
  <FormSelect
    :id="id"
    :label="t(label)"
    :model-value="modelValue"
    :options="options"
    :placeholder="t(placeholder)"
    @update:model-value="$emit('update:model-value', $event)"
  />
</template>
