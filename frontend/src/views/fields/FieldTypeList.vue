<script setup lang="ts">
import type { SelectOption } from "logitar-vue3-ui";
import { arrayUtils, objectUtils } from "logitar-js";
import { computed, inject, ref, watch } from "vue";
import { parsingUtils } from "logitar-js";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import AppBreadcrumb from "@/components/shared/AppBreadcrumb.vue";
import AppPagination from "@/components/shared/AppPagination.vue";
import CountSelect from "@/components/shared/CountSelect.vue";
import CreateFieldType from "@/components/fields/CreateFieldType.vue";
import DataTypeSelect from "@/components/fields/DataTypeSelect.vue";
import EditIcon from "@/components/shared/EditIcon.vue";
import RefreshButton from "@/components/shared/RefreshButton.vue";
import SearchInput from "@/components/shared/SearchInput.vue";
import SortSelect from "@/components/shared/SortSelect.vue";
import StatusBlock from "@/components/shared/StatusBlock.vue";
import type { DataType, FieldType, FieldTypeSort, SearchFieldTypesPayload } from "@/types/fields";
import type { SearchResults } from "@/types/search";
import { handleErrorKey } from "@/inject/App";
import { searchFieldTypes } from "@/api/fields/types";
import { useToastStore } from "@/stores/toast";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const toasts = useToastStore();
const { isEmpty } = objectUtils;
const { orderBy } = arrayUtils;
const { parseBoolean, parseNumber } = parsingUtils;
const { rt, t, tm } = useI18n();

const fieldTypes = ref<FieldType[]>([]);
const isLoading = ref<boolean>(false);
const timestamp = ref<number>(0);
const total = ref<number>(0);

const count = computed<number>(() => parseNumber(route.query.count?.toString()) || 10);
const isDescending = computed<boolean>(() => parseBoolean(route.query.isDescending?.toString()) ?? false);
const page = computed<number>(() => parseNumber(route.query.page?.toString()) || 1);
const search = computed<string>(() => route.query.search?.toString() ?? "");
const sort = computed<string>(() => route.query.sort?.toString() ?? "");
const type = computed<string>(() => route.query.type?.toString() ?? "");

const sortOptions = computed<SelectOption[]>(() =>
  orderBy(
    Object.entries(tm(rt("fields.type.sort.options"))).map(([value, text]) => ({ text, value }) as SelectOption),
    "text",
  ),
);

async function refresh(): Promise<void> {
  const payload: SearchFieldTypesPayload = {
    dataType: type.value ? (type.value as DataType) : undefined,
    ids: [],
    search: {
      terms: search.value
        .split(" ")
        .filter((term) => term.length > 0)
        .map((term) => ({ value: `%${term}%` })),
      operator: "And",
    },
    sort: sort.value ? [{ field: sort.value as FieldTypeSort, isDescending: isDescending.value }] : [],
    skip: (page.value - 1) * count.value,
    limit: count.value,
  };
  isLoading.value = true;
  const now = Date.now();
  timestamp.value = now;
  try {
    const results: SearchResults<FieldType> = await searchFieldTypes(payload);
    if (now === timestamp.value) {
      fieldTypes.value = results.items;
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
    case "type":
    case "count":
      query.page = "1";
      break;
  }
  router.replace({ ...route, query });
}

function onCreated(fieldType: FieldType) {
  toasts.success("fields.type.created");
  router.push({ name: "FieldTypeEdit", params: { id: fieldType.id } });
}

watch(
  () => route,
  (route) => {
    if (route.name === "FieldTypeList") {
      const { query } = route;
      if (!query.page || !query.count) {
        router.replace({
          ...route,
          query: isEmpty(query)
            ? {
                search: "",
                type: "",
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
    <h1>{{ t("fields.type.title") }}</h1>
    <AppBreadcrumb :current="t('fields.type.title')" />
    <div class="my-3">
      <RefreshButton class="me-1" :disabled="isLoading" :loading="isLoading" @click="refresh()" />
      <CreateFieldType class="ms-1" @created="onCreated" @error="handleError" />
    </div>
    <div class="mb-3 row">
      <SearchInput class="col" :model-value="search" @update:model-value="setQuery('search', $event)" />
      <DataTypeSelect class="col" :model-value="type" @update:model-value="setQuery('type', $event)" />
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
    <template v-if="fieldTypes.length">
      <table class="table table-striped">
        <thead>
          <tr>
            <th scope="col">{{ t("fields.type.sort.options.UniqueName") }}</th>
            <th scope="col">{{ t("fields.type.sort.options.DisplayName") }}</th>
            <th scope="col">{{ t("fields.type.dataType.label") }}</th>
            <th scope="col">{{ t("fields.type.sort.options.UpdatedOn") }}</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="fieldType in fieldTypes" :key="fieldType.id">
            <td>
              <RouterLink :to="{ name: 'FieldTypeEdit', params: { id: fieldType.id } }"><EditIcon /> {{ fieldType.uniqueName }}</RouterLink>
            </td>
            <td>
              <template v-if="fieldType.displayName">{{ fieldType.displayName }}</template>
              <span v-else class="text-muted">{{ "—" }}</span>
            </td>
            <td>{{ t(`fields.type.dataType.options.${fieldType.dataType}`) }}</td>
            <td><StatusBlock :actor="fieldType.updatedBy" :date="fieldType.updatedOn" /></td>
          </tr>
        </tbody>
      </table>
      <AppPagination :count="count" :model-value="page" :total="total" @update:model-value="setQuery('page', $event.toString())" />
    </template>
    <p v-else>{{ t("fields.type.empty") }}</p>
  </main>
</template>
