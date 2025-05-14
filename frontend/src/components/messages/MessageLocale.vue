<script setup lang="ts">
import { TarInput } from "logitar-vue3-ui";
import { useI18n } from "vue-i18n";

import type { Locale } from "@/types/i18n";
import { formatLocale } from "@/helpers/format";
import { computed } from "vue";

const { t } = useI18n();

const props = withDefaults(
  defineProps<{
    disabled?: boolean | string;
    floating?: boolean | string;
    id?: string;
    label?: string;
    locale?: Locale | null;
    placeholder?: string;
  }>(),
  {
    disabled: true,
    floating: true,
    id: "locale",
    label: "locale.label",
  },
);

const value = computed<string>(() => (props.locale ? formatLocale(props.locale) : ""));
</script>

<template>
  <TarInput :disabled="disabled" :floating="floating" :id="id" :label="t(label)" :model-value="value" :placeholder="t(placeholder ?? label)" />
</template>
