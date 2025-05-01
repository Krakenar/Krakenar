<script setup lang="ts">
import type { SelectOption } from "logitar-vue3-ui";
import { arrayUtils } from "logitar-js";
import { computed, ref } from "vue";
import { useI18n } from "vue-i18n";

import FormSelect from "@/components/forms/FormSelect.vue";
import countries from "@/resources/countries.json";
import type { Country } from "@/types/users";

const { orderBy } = arrayUtils;
const { t } = useI18n();

const props = withDefaults(
  defineProps<{
    id?: string;
    label?: string;
    modelValue?: string;
    placeholder?: string;
    required?: boolean | string;
  }>(),
  {
    id: "country",
    label: "users.address.country.label",
    placeholder: "users.address.country.placeholder",
  },
);

const country = ref<Country | undefined>(props.modelValue ? countries.find(({ code }) => code === props.modelValue) : undefined);

const options = computed<SelectOption[]>(() =>
  orderBy(
    countries.map(({ code }) => ({ value: code, text: t(`countries.${code}.name`) })),
    "text",
  ),
);

const emit = defineEmits<{
  (e: "selected", value: Country): void;
  (e: "update:model-value", value: string): void;
}>();

function onModelValueUpdate(code: string): void {
  emit("update:model-value", code);

  country.value = countries.find((country) => country.code === code) as Country;
  emit("selected", country.value);
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
    <template #append>
      <span v-if="country" class="input-group-text">
        <img :src="`/img/countries/${country.code}.svg`" :alt="country.flags.alt" width="48" />
      </span>
    </template>
  </FormSelect>
</template>
