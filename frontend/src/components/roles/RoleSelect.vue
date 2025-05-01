<script setup lang="ts">
import { TarSelect, type SelectOption } from "logitar-vue3-ui";
import { arrayUtils, parsingUtils } from "logitar-js";
import { computed, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";

import type { Role, SearchRolesPayload } from "@/types/roles";
import type { SearchResults } from "@/types/search";
import { formatRole } from "@/helpers/format";
import { searchRoles } from "@/api/roles";

const { orderBy } = arrayUtils;
const { parseBoolean } = parsingUtils;
const { t } = useI18n();

const props = withDefaults(
  defineProps<{
    disabled?: boolean | string;
    exclude?: string[];
    id?: string;
    label?: string;
    modelValue?: string;
    placeholder?: string;
  }>(),
  {
    exclude: () => [],
    id: "role",
    label: "roles.select.label",
    placeholder: "roles.select.placeholder",
  },
);

const roles = ref<Role[]>([]);

const isDisabled = computed<boolean>(() => parseBoolean(props.disabled) || options.value.length === 0);
const options = computed<SelectOption[]>(() =>
  orderBy(
    roles.value.filter(({ id }) => !props.exclude.includes(id)).map((role) => ({ text: formatRole(role), value: role.id })),
    "text",
  ),
);

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "selected", value: Role | undefined): void;
  (e: "update:modelValue", value: string): void;
}>();

function onModelValueUpdate(id: string): void {
  emit("update:modelValue", id);

  const role: Role | undefined = roles.value.find((role) => role.id === id);
  emit("selected", role);
}

onMounted(async () => {
  try {
    const payload: SearchRolesPayload = {
      ids: [],
      search: { terms: [], operator: "And" },
      sort: [],
      skip: 0,
      limit: 0,
    };
    const results: SearchResults<Role> = await searchRoles(payload);
    roles.value = results.items;
  } catch (e: unknown) {
    emit("error", e);
  }
});
</script>

<template>
  <TarSelect
    :disabled="isDisabled"
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
