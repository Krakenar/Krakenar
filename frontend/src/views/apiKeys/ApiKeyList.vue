<script setup lang="ts">
import type { SelectOption } from "logitar-vue3-ui";
import { arrayUtils, objectUtils } from "logitar-js";
import { computed, inject, ref, watch } from "vue";
import { parsingUtils } from "logitar-js";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import AppPagination from "@/components/shared/AppPagination.vue";
import CountSelect from "@/components/shared/CountSelect.vue";
import CreateApiKey from "@/components/apiKeys/CreateApiKey.vue";
import EditIcon from "@/components/shared/EditIcon.vue";
import ExpiredBadge from "@/components/apiKeys/ExpiredBadge.vue";
import RefreshButton from "@/components/shared/RefreshButton.vue";
import RoleSelect from "@/components/roles/RoleSelect.vue";
import SearchInput from "@/components/shared/SearchInput.vue";
import SortSelect from "@/components/shared/SortSelect.vue";
import StatusBlock from "@/components/shared/StatusBlock.vue";
import YesNoSelect from "@/components/shared/YesNoSelect.vue";
import type { ApiKey, ApiKeySort, SearchApiKeysPayload } from "@/types/apiKeys";
import type { SearchResults } from "@/types/search";
import { handleErrorKey } from "@/inject/App";
import { isExpired as isApiKeyExpired } from "@/helpers/apiKeys";
import { searchApiKeys } from "@/api/apiKeys";
import { useToastStore } from "@/stores/toast";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const toasts = useToastStore();
const { d, rt, t, tm } = useI18n();
const { isEmpty } = objectUtils;
const { orderBy } = arrayUtils;
const { parseBoolean, parseNumber } = parsingUtils;

const apiKeys = ref<ApiKey[]>([]);
const isLoading = ref<boolean>(false);
const timestamp = ref<number>(0);
const total = ref<number>(0);

const count = computed<number>(() => parseNumber(route.query.count?.toString()) || 10);
const hasAuthenticated = computed<boolean | undefined>(() => parseBoolean(route.query.authenticated?.toString()));
const isDescending = computed<boolean>(() => parseBoolean(route.query.isDescending?.toString()) ?? false);
const isExpired = computed<boolean | undefined>(() => parseBoolean(route.query.expired?.toString()));
const page = computed<number>(() => parseNumber(route.query.page?.toString()) || 1);
const roleId = computed<string>(() => route.query.role?.toString() ?? "");
const search = computed<string>(() => route.query.search?.toString() ?? "");
const sort = computed<string>(() => route.query.sort?.toString() ?? "");

const sortOptions = computed<SelectOption[]>(() =>
  orderBy(
    Object.entries(tm(rt("apiKeys.sort.options"))).map(([value, text]) => ({ text, value }) as SelectOption),
    "text",
  ),
);

async function refresh(): Promise<void> {
  const payload: SearchApiKeysPayload = {
    hasAuthenticated: hasAuthenticated.value,
    ids: [],
    roleId: roleId.value,
    search: {
      terms: search.value
        .split(" ")
        .filter((term) => term.length > 0)
        .map((term) => ({ value: `%${term}%` })),
      operator: "And",
    },
    status: typeof isExpired.value === "undefined" ? undefined : { isExpired: isExpired.value },
    sort: sort.value ? [{ field: sort.value as ApiKeySort, isDescending: isDescending.value }] : [],
    skip: (page.value - 1) * count.value,
    limit: count.value,
  };
  isLoading.value = true;
  const now = Date.now();
  timestamp.value = now;
  try {
    const results: SearchResults<ApiKey> = await searchApiKeys(payload);
    if (now === timestamp.value) {
      apiKeys.value = results.items;
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
    case "authenticated":
    case "expired":
    case "role":
    case "search":
    case "count":
      query.page = "1";
      break;
  }
  router.replace({ ...route, query });
}

function onCreated(apiKey: ApiKey) {
  toasts.success("apiKeys.created");
  router.push({ name: "ApiKeyEdit", params: { id: apiKey.id }, query: { "x-api-key": apiKey.xApiKey } });
}

watch(
  () => route,
  (route) => {
    if (route.name === "ApiKeyList") {
      const { query } = route;
      if (!query.page || !query.count) {
        router.replace({
          ...route,
          query: isEmpty(query)
            ? {
                authenticated: "",
                expired: "",
                role: "",
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
    <h1>{{ t("apiKeys.title") }}</h1>
    <div class="my-3">
      <RefreshButton class="me-1" :disabled="isLoading" :loading="isLoading" @click="refresh()" />
      <CreateApiKey class="ms-1" @created="onCreated" @error="handleError" />
    </div>
    <div class="mb-3 row">
      <YesNoSelect
        class="col"
        id="expired"
        label="apiKeys.expired"
        :model-value="isExpired"
        @update:model-value="setQuery('expired', $event?.toString() ?? '')"
      />
      <RoleSelect class="col" :model-value="roleId" @update:model-value="setQuery('role', $event?.toString() ?? '')" />
      <YesNoSelect
        class="col"
        id="authenticated"
        label="apiKeys.authenticated"
        :model-value="hasAuthenticated"
        @update:model-value="setQuery('authenticated', $event?.toString() ?? '')"
      />
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
    <template v-if="apiKeys.length">
      <table class="table table-striped">
        <thead>
          <tr>
            <th scope="col">{{ t("apiKeys.sort.options.Name") }}</th>
            <th scope="col">{{ t("apiKeys.sort.options.ExpiresOn") }}</th>
            <th scope="col">{{ t("apiKeys.sort.options.AuthenticatedOn") }}</th>
            <th scope="col">{{ t("apiKeys.sort.options.UpdatedOn") }}</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="apiKey in apiKeys" :key="apiKey.id">
            <td>
              <RouterLink :to="{ name: 'ApiKeyEdit', params: { id: apiKey.id } }"><EditIcon /> {{ apiKey.name }}</RouterLink>
            </td>
            <td>
              <ExpiredBadge v-if="isApiKeyExpired(apiKey)" />
              <template v-else-if="apiKey.expiresOn">{{ d(apiKey.expiresOn, "medium") }}</template>
              <span v-else class="text-muted">{{ "—" }}</span>
            </td>
            <td>
              <template v-if="apiKey.authenticatedOn">{{ d(apiKey.authenticatedOn, "medium") }}</template>
              <span v-else class="text-muted">{{ "—" }}</span>
            </td>
            <td><StatusBlock :actor="apiKey.updatedBy" :date="apiKey.updatedOn" /></td>
          </tr>
        </tbody>
      </table>
      <AppPagination :count="count" :model-value="page" :total="total" @update:model-value="setQuery('page', $event.toString())" />
    </template>
    <p v-else>{{ t("apiKeys.empty") }}</p>
  </main>
</template>
