<script setup lang="ts">
import type { SelectOption } from "logitar-vue3-ui";
import { arrayUtils } from "logitar-js";
import { computed, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";

import AppSelect from "@/components/shared/AppSelect.vue";
import type { SearchResults } from "@/types/search";
import type { SearchUsersPayload, User } from "@/types/users";
import { formatUser } from "@/helpers/format";
import { searchUsers } from "@/api/users";

const { orderBy } = arrayUtils;
const { t } = useI18n();

withDefaults(
  defineProps<{
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
  <AppSelect :id="id" :label="t(label)" :model-value="modelValue" :options="options" :placeholder="t(placeholder)" @update:model-value="onUserSelected" />
</template>
