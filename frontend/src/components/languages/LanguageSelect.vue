<script setup lang="ts">
import { TarSelect, type SelectOption } from "logitar-vue3-ui";
import { arrayUtils } from "logitar-js";
import { computed, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";

import type { Language, SearchLanguagesPayload } from "@/types/languages";
import type { SearchResults } from "@/types/search";
import { formatLocale } from "@/helpers/format";
import { searchLanguages } from "@/api/languages";

const { orderBy } = arrayUtils;
const { t } = useI18n();

const props = withDefaults(
  defineProps<{
    exclude?: string[];
    id?: string;
    label?: string;
    modelValue?: string;
    placeholder?: string;
  }>(),
  {
    id: "language",
    label: "languages.select.label",
    placeholder: "languages.select.placeholder",
  },
);

const languages = ref<Language[]>([]);

const options = computed<SelectOption[]>(() =>
  orderBy(
    languages.value
      .filter((language) => !props.exclude || !props.exclude.includes(language.id))
      .map((language) => ({ text: formatLocale(language.locale), value: language.id })),
    "text",
  ),
);

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "selected", value: Language | undefined): void;
  (e: "update:modelValue", value: string): void;
}>();

function onModelValueUpdate(id: string): void {
  emit("update:modelValue", id);

  const language: Language | undefined = languages.value.find((language) => language.id === id);
  emit("selected", language);
}

onMounted(async () => {
  try {
    const payload: SearchLanguagesPayload = {
      ids: [],
      search: { terms: [], operator: "And" },
      sort: [],
      skip: 0,
      limit: 0,
    };
    const results: SearchResults<Language> = await searchLanguages(payload);
    languages.value = results.items;
  } catch (e: unknown) {
    emit("error", e);
  }
});
</script>

<template>
  <TarSelect
    :disabled="options.length < 1"
    floating
    :id="id"
    :label="t(label)"
    :model-value="modelValue"
    :options="options"
    :placeholder="t(placeholder ?? label)"
    @update:modelValue="onModelValueUpdate"
  >
    <template #append>
      <slot name="append"></slot>
    </template>
  </TarSelect>
</template>
