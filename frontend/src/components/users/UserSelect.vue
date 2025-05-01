<script setup lang="ts">
import { TarSelect, type SelectOption } from "logitar-vue3-ui";
import { arrayUtils, parsingUtils } from "logitar-js";
import { computed, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";

import type { SearchResults } from "@/types/search";
import type { SearchUsersPayload, User } from "@/types/users";
import { formatUser } from "@/helpers/format";
import { searchUsers } from "@/api/users";

const { orderBy } = arrayUtils;
const { parseBoolean } = parsingUtils;
const { t } = useI18n();

const props = withDefaults(
  defineProps<{
    disabled?: boolean | string;
    id?: string;
    label?: string;
    modelValue?: string;
    placeholder?: string;
  }>(),
  {
    id: "user",
    label: "users.select.label",
    placeholder: "users.select.placeholder",
  },
);

const users = ref<User[]>([]);

const isDisabled = computed<boolean>(() => parseBoolean(props.disabled) || options.value.length === 0);
const options = computed<SelectOption[]>(() =>
  orderBy(
    users.value.map((user) => ({ text: formatUser(user), value: user.id })),
    "text",
  ),
);

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "selected", value: User | undefined): void;
  (e: "update:model-value", value: string): void;
}>();

function onUserSelected(id: string): void {
  emit("update:model-value", id);

  const user: User | undefined = users.value.find((user) => user.id === id);
  emit("selected", user);
}

onMounted(async () => {
  try {
    const payload: SearchUsersPayload = {
      ids: [],
      search: { terms: [], operator: "And" },
      sort: [],
      skip: 0,
      limit: 0,
    };
    const results: SearchResults<User> = await searchUsers(payload);
    users.value = results.items;
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
    :placeholder="t(placeholder)"
    @update:model-value="onUserSelected"
  />
</template>
