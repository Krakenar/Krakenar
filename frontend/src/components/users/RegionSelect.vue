<script setup lang="ts">
import type { SelectOption } from "logitar-vue3-ui";
import { arrayUtils } from "logitar-js";
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import FormSelect from "@/components/forms/FormSelect.vue";
import type { Country } from "@/types/users";

const { orderBy } = arrayUtils;
const { t } = useI18n();

const props = withDefaults(
  defineProps<{
    country?: Country;
    id?: string;
    label?: string;
    modelValue?: string;
    placeholder?: string;
    required?: boolean | string;
  }>(),
  {
    id: "region",
    label: "users.address.region.label",
    placeholder: "users.address.region.placeholder",
  },
);

const options = computed<SelectOption[]>(() =>
  orderBy(props.country?.regions?.map((region) => ({ value: region, text: t(`countries.${props.country?.code}.regions.${region}`) })) ?? [], "text"),
);

defineEmits<{
  (e: "update:model-value", value: string): void;
}>();
</script>

<template>
  <FormSelect
    :disabled="options.length < 0"
    :id="id"
    :label="t(label)"
    :model-value="modelValue"
    :options="options"
    :placeholder="t(placeholder)"
    :required="required"
    @update:model-value="$emit('update:model-value', $event)"
  />
</template>
