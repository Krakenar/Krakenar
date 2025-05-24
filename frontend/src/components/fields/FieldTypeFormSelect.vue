<script setup lang="ts">
import type { SelectOption } from "logitar-vue3-ui";
import { arrayUtils } from "logitar-js";
import { computed, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";

import FormSelect from "@/components/forms/FormSelect.vue";
import type { FieldType, SearchFieldTypesPayload } from "@/types/fields";
import type { SearchResults } from "@/types/search";
import { formatFieldType } from "@/helpers/format";
import { searchFieldTypes } from "@/api/fields/types";

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
    id: "field-type",
    label: "fields.type.select.label",
    placeholder: "fields.type.select.placeholder",
  },
);

const fieldTypes = ref<FieldType[]>([]);

const options = computed<SelectOption[]>(() =>
  orderBy(
    fieldTypes.value.map((fieldType) => ({ text: formatFieldType(fieldType), value: fieldType.id })),
    "text",
  ),
);

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "selected", value: FieldType | undefined): void;
  (e: "update:modelValue", value: string): void;
}>();

function onModelValueUpdate(id: string): void {
  emit("update:modelValue", id);

  const fieldType: FieldType | undefined = fieldTypes.value.find((fieldType) => fieldType.id === id);
  emit("selected", fieldType);
}

onMounted(async () => {
  try {
    const payload: SearchFieldTypesPayload = {
      ids: [],
      search: { terms: [], operator: "And" },
      sort: [],
      skip: 0,
      limit: 0,
    };
    const results: SearchResults<FieldType> = await searchFieldTypes(payload);
    fieldTypes.value = results.items;
  } catch (e: unknown) {
    emit("error", e);
  }
});
</script>

<template>
  <FormSelect
    :described-by="`${id}-help`"
    :disabled="options.length < 1"
    :id="id"
    :label="t(label)"
    :model-value="modelValue"
    :options="options"
    :placeholder="t(placeholder ?? label)"
    :required="required"
    @update:modelValue="onModelValueUpdate"
  >
    <template #after>
      <div :id="`${id}-help`" class="form-text"><font-awesome-icon icon="fas fa-triangle-exclamation" /> {{ t("fields.type.select.help") }}</div>
    </template>
  </FormSelect>
</template>
