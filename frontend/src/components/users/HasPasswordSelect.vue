<script setup lang="ts">
import { TarSelect, type SelectOption } from "logitar-vue3-ui";
import { arrayUtils, parsingUtils } from "logitar-js";
import { computed } from "vue";
import { useI18n } from "vue-i18n";

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
    id: "has-password",
    label: "users.password.label",
    placeholder: "placeholder",
  },
);

const options = computed<SelectOption[]>(() =>
  orderBy(
    [
      {
        text: t("users.password.less"),
        value: "false",
      },
      {
        text: t("users.password.with"),
        value: "true",
      },
    ] as SelectOption[],
    "text",
  ),
);

defineEmits<{
  (e: "update:model-value", value: boolean | undefined): void;
}>();
</script>

<template>
  <TarSelect
    floating
    :id="id"
    :label="t(label)"
    :model-value="modelValue?.toString()"
    :options="options"
    :placeholder="t(placeholder ?? label)"
    @update:model-value="$emit('update:model-value', parseBoolean($event))"
  />
</template>
