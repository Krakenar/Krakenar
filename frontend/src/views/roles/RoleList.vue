<script setup lang="ts">
import type { SelectOption } from "logitar-vue3-ui";
import { arrayUtils, objectUtils } from "logitar-js";
import { computed, inject, ref, watch } from "vue";
import { parsingUtils } from "logitar-js";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import AppPagination from "@/components/shared/AppPagination.vue";
import CountSelect from "@/components/shared/CountSelect.vue";
import CreateRole from "@/components/roles/CreateRole.vue";
import EditIcon from "@/components/shared/EditIcon.vue";
import RefreshButton from "@/components/shared/RefreshButton.vue";
import SearchInput from "@/components/shared/SearchInput.vue";
import SortSelect from "@/components/shared/SortSelect.vue";
import StatusBlock from "@/components/shared/StatusBlock.vue";
import type { Role, RoleSort, SearchRolesPayload } from "@/types/roles";
import type { SearchResults } from "@/types/search";
import { handleErrorKey } from "@/inject/App";
import { searchRoles } from "@/api/roles";
import { useToastStore } from "@/stores/toast";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const toasts = useToastStore();
const { isEmpty } = objectUtils;
const { orderBy } = arrayUtils;
const { parseBoolean, parseNumber } = parsingUtils;
const { rt, t, tm } = useI18n();

const isLoading = ref<boolean>(false);
const roles = ref<Role[]>([]);
const timestamp = ref<number>(0);
const total = ref<number>(0);

const count = computed<number>(() => parseNumber(route.query.count?.toString()) || 10);
const isDescending = computed<boolean>(() => parseBoolean(route.query.isDescending?.toString()) ?? false);
const page = computed<number>(() => parseNumber(route.query.page?.toString()) || 1);
const search = computed<string>(() => route.query.search?.toString() ?? "");
const sort = computed<string>(() => route.query.sort?.toString() ?? "");

const sortOptions = computed<SelectOption[]>(() =>
  orderBy(
    Object.entries(tm(rt("roles.sort.options"))).map(([value, text]) => ({ text, value }) as SelectOption),
    "text",
  ),
);

async function refresh(): Promise<void> {
  const payload: SearchRolesPayload = {
    ids: [],
    search: {
      terms: search.value
        .split(" ")
        .filter((term) => term.length > 0)
        .map((term) => ({ value: `%${term}%` })),
      operator: "And",
    },
    sort: sort.value ? [{ field: sort.value as RoleSort, isDescending: isDescending.value }] : [],
    skip: (page.value - 1) * count.value,
    limit: count.value,
  };
  isLoading.value = true;
  const now = Date.now();
  timestamp.value = now;
  try {
    const results: SearchResults<Role> = await searchRoles(payload);
    if (now === timestamp.value) {
      roles.value = results.items;
      total.value = results.total;
    }
  } catch (e: unknown) {
    handleError(e);
  } finally {
    if (now === timestamp.value) {
      isLoading.value = false;
    }
  }
}

function setQuery(key: string, value: string): void {
  const query = { ...route.query, [key]: value };
  switch (key) {
    case "search":
    case "count":
      query.page = "1";
      break;
  }
  router.replace({ ...route, query });
}

function onCreated(role: Role) {
  toasts.success("roles.created");
  router.push({ name: "RoleEdit", params: { id: role.id } });
}

watch(
  () => route,
  (route) => {
    if (route.name === "RoleList") {
      const { query } = route;
      if (!query.page || !query.count) {
        router.replace({
          ...route,
          query: isEmpty(query)
            ? {
                search: "",
                sort: "UpdatedOn",
                isDescending: "true",
                page: 1,
                count: 10,
              }
            : {
                page: 1,
                count: 10,
                ...query,
              },
        });
      } else {
        refresh();
      }
    }
  },
  { deep: true, immediate: true },
);
</script>

<template>
  <main class="container">
    <h1>{{ t("roles.title") }}</h1>
    <div class="my-3">
      <RefreshButton class="me-1" :disabled="isLoading" :loading="isLoading" @click="refresh()" />
      <CreateRole class="ms-1" @created="onCreated" @error="handleError" />
    </div>
    <div class="mb-3 row">
      <SearchInput class="col" :model-value="search" @update:model-value="setQuery('search', $event)" />
      <SortSelect
        class="col"
        :descending="isDescending"
        :model-value="sort"
        :options="sortOptions"
        @descending="setQuery('isDescending', $event.toString())"
        @update:model-value="setQuery('sort', $event)"
      />
      <CountSelect class="col" :model-value="count" @update:model-value="setQuery('count', ($event ?? 10).toString())" />
    </div>
    <template v-if="roles.length">
      <table class="table table-striped">
        <thead>
          <tr>
            <th scope="col">{{ t("roles.sort.options.UniqueName") }}</th>
            <th scope="col">{{ t("roles.sort.options.DisplayName") }}</th>
            <th scope="col">{{ t("roles.sort.options.UpdatedOn") }}</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="role in roles" :key="role.id">
            <td>
              <RouterLink :to="{ name: 'RoleEdit', params: { id: role.id } }"><EditIcon /> {{ role.uniqueName }}</RouterLink>
            </td>
            <td>
              <template v-if="role.displayName">{{ role.displayName }}</template>
              <span v-else class="text-muted">{{ "—" }}</span>
            </td>
            <td><StatusBlock :actor="role.updatedBy" :date="role.updatedOn" /></td>
          </tr>
        </tbody>
      </table>
      <AppPagination :count="count" :model-value="page" :total="total" @update:model-value="setQuery('page', $event.toString())" />
    </template>
    <p v-else>{{ t("roles.empty") }}</p>
  </main>
</template>
