<script setup lang="ts">
import type { SelectOption } from "logitar-vue3-ui";
import { arrayUtils } from "logitar-js";
import { computed, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";

import FormSelect from "@/components/forms/FormSelect.vue";
import type { ContentType, SearchContentTypesPayload } from "@/types/contents";
import type { SearchResults } from "@/types/search";
import { formatContentType } from "@/helpers/format";
import { searchContentTypes } from "@/api/contents/types";

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
    id: "content-type",
    label: "contents.type.select.label",
    placeholder: "contents.type.select.placeholder",
  },
);

const contentTypes = ref<ContentType[]>([]);

const options = computed<SelectOption[]>(() =>
  orderBy(
    contentTypes.value.map((contentType) => ({ text: formatContentType(contentType), value: contentType.id })),
    "text",
  ),
);

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "selected", value: ContentType | undefined): void;
  (e: "update:modelValue", value: string): void;
}>();

function onModelValueUpdate(id: string): void {
  emit("update:modelValue", id);

  const contentType: ContentType | undefined = contentTypes.value.find((contentType) => contentType.id === id);
  emit("selected", contentType);
}

onMounted(async () => {
  try {
    const payload: SearchContentTypesPayload = {
      ids: [],
      search: { terms: [], operator: "And" },
      sort: [],
      skip: 0,
      limit: 0,
    };
    const results: SearchResults<ContentType> = await searchContentTypes(payload);
    contentTypes.value = results.items;
  } catch (e: unknown) {
    emit("error", e);
  }
});
</script>

<template>
  <FormSelect
    :disabled="options.length < 1"
    :id="id"
    :label="t(label)"
    :model-value="modelValue"
    :options="options"
    :placeholder="t(placeholder ?? label)"
    :required="required"
    @update:modelValue="onModelValueUpdate"
  />
</template>
