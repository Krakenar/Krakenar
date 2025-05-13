<script setup lang="ts">
import type { SelectOption } from "logitar-vue3-ui";
import { arrayUtils } from "logitar-js";
import { computed, ref, watch } from "vue";
import { useI18n } from "vue-i18n";

import FormSelect from "@/components/forms/FormSelect.vue";
import type { ContentType, Template, SearchTemplatesPayload } from "@/types/templates";
import type { SearchResults } from "@/types/search";
import { formatTemplate } from "@/helpers/format";
import { searchTemplates } from "@/api/templates";

const { orderBy } = arrayUtils;
const { t } = useI18n();

const props = withDefaults(
  defineProps<{
    contentType?: ContentType;
    id?: string;
    label?: string;
    modelValue?: string;
    placeholder?: string;
    required?: boolean | string;
  }>(),
  {
    id: "template",
    label: "templates.select.label",
    placeholder: "templates.select.placeholder",
  },
);

const templates = ref<Template[]>([]);

const options = computed<SelectOption[]>(() =>
  orderBy(
    templates.value.map((template) => ({ text: formatTemplate(template), value: template.id })),
    "text",
  ),
);

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "selected", value: Template | undefined): void;
  (e: "update:modelValue", value: string): void;
}>();

function onModelValueUpdate(id: string): void {
  emit("update:modelValue", id);

  const template: Template | undefined = templates.value.find((template) => template.id === id);
  emit("selected", template);
}

async function refresh(contentType?: ContentType): Promise<void> {
  try {
    const payload: SearchTemplatesPayload = {
      contentType,
      ids: [],
      search: { terms: [], operator: "And" },
      sort: [],
      skip: 0,
      limit: 0,
    };
    const results: SearchResults<Template> = await searchTemplates(payload);
    templates.value = results.items;
  } catch (e: unknown) {
    emit("error", e);
  }
}

watch(() => props.contentType, refresh, { immediate: true });
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
