<script setup lang="ts">
import type { SelectOption } from "logitar-vue3-ui";
import { arrayUtils } from "logitar-js";
import { computed, ref, watch } from "vue";

import MultiSelect from "@/components/fields/MultiSelect.vue";
import type { ContentLocale, SearchContentLocalesPayload } from "@/types/contents";
import type { SearchResults } from "@/types/search";
import { formatContentLocale } from "@/helpers/format";
import { searchContentLocales } from "@/api/contents/items";

const { orderBy } = arrayUtils;

const props = withDefaults(
  defineProps<{
    contentType?: string;
    describedBy?: string;
    id?: string;
    label?: string;
    language?: string;
    modelValue?: string[];
    name?: string;
    placeholder?: string;
  }>(),
  {
    id: "content",
    label: "contents.item.select.label",
    modelValue: () => [],
    placeholder: "contents.item.select.placeholder",
  },
);

const locales = ref<ContentLocale[]>([]);

const options = computed<SelectOption[]>(() =>
  orderBy(
    locales.value.map((locale) => ({ text: formatContentLocale(locale), value: locale.content?.id })),
    "text",
  ),
);

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "update:modelValue", value: string[]): void;
}>();

async function refresh(contentTypeId?: string, languageId?: string): Promise<void> {
  try {
    const payload: SearchContentLocalesPayload = {
      contentTypeId,
      ids: [],
      languageId,
      search: { terms: [], operator: "And" },
      sort: [],
      skip: 0,
      limit: 0,
    };
    const results: SearchResults<ContentLocale> = await searchContentLocales(payload);
    locales.value = results.items;
  } catch (e: unknown) {
    emit("error", e);
  }
}
watch(
  () => props.contentType,
  (contentType) => refresh(contentType, props.language),
  { immediate: true },
);
watch(
  () => props.language,
  (language) => refresh(props.contentType, language),
  { immediate: true },
);
</script>

<template>
  <MultiSelect
    :described-by="describedBy"
    :id="id"
    :label="label"
    :model-value="modelValue"
    :name="name"
    :options="options"
    :placeholder="placeholder"
    @update:modelValue="$emit('update:modelValue', $event)"
  >
    <template #label-override>
      <slot name="label-override"></slot>
    </template>
  </MultiSelect>
</template>
