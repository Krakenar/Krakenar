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
import CreateContentType from "@/components/contents/CreateContentType.vue";
import EditIcon from "@/components/shared/EditIcon.vue";
import RefreshButton from "@/components/shared/RefreshButton.vue";
import SearchInput from "@/components/shared/SearchInput.vue";
import SortSelect from "@/components/shared/SortSelect.vue";
import StatusBlock from "@/components/shared/StatusBlock.vue";
import YesNoSelect from "@/components/shared/YesNoSelect.vue";
import type { ContentType, ContentTypeSort, SearchContentTypesPayload } from "@/types/contents";
import type { SearchResults } from "@/types/search";
import { handleErrorKey } from "@/inject/App";
import { searchContentTypes } from "@/api/contents";
import { useToastStore } from "@/stores/toast";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const toasts = useToastStore();
const { isEmpty } = objectUtils;
const { orderBy } = arrayUtils;
const { parseBoolean, parseNumber } = parsingUtils;
const { rt, t, tm } = useI18n();

const contentTypes = ref<ContentType[]>([]);
const isLoading = ref<boolean>(false);
const timestamp = ref<number>(0);
const total = ref<number>(0);

const count = computed<number>(() => parseNumber(route.query.count?.toString()) || 10);
const isDescending = computed<boolean>(() => parseBoolean(route.query.isDescending?.toString()) ?? false);
const isInvariant = computed<boolean | undefined>(() => parseBoolean(route.query.invariant?.toString()));
const page = computed<number>(() => parseNumber(route.query.page?.toString()) || 1);
const search = computed<string>(() => route.query.search?.toString() ?? "");
const sort = computed<string>(() => route.query.sort?.toString() ?? "");

const sortOptions = computed<SelectOption[]>(() =>
  orderBy(
    Object.entries(tm(rt("contents.type.sort.options"))).map(([value, text]) => ({ text, value }) as SelectOption),
    "text",
  ),
);

async function refresh(): Promise<void> {
  const payload: SearchContentTypesPayload = {
    ids: [],
    isInvariant: isInvariant.value,
    search: {
      terms: search.value
        .split(" ")
        .filter((term) => term.length > 0)
        .map((term) => ({ value: `%${term}%` })),
      operator: "And",
    },
    sort: sort.value ? [{ field: sort.value as ContentTypeSort, isDescending: isDescending.value }] : [],
    skip: (page.value - 1) * count.value,
    limit: count.value,
  };
  isLoading.value = true;
  const now = Date.now();
  timestamp.value = now;
  try {
    const results: SearchResults<ContentType> = await searchContentTypes(payload);
    if (now === timestamp.value) {
      contentTypes.value = results.items;
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
    case "invariant":
    case "search":
    case "count":
      query.page = "1";
      break;
  }
  router.replace({ ...route, query });
}

function onCreated(contentType: ContentType) {
  toasts.success("contents.type.created");
  router.push({ name: "ContentTypeEdit", params: { id: contentType.id } });
}

watch(
  () => route,
  (route) => {
    if (route.name === "ContentTypeList") {
      const { query } = route;
      if (!query.page || !query.count) {
        router.replace({
          ...route,
          query: isEmpty(query)
            ? {
                invariant: "",
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
    <h1>{{ t("contents.type.title") }}</h1>
    <AppBreadcrumb :current="t('contents.type.title')" />
    <div class="my-3">
      <RefreshButton class="me-1" :disabled="isLoading" :loading="isLoading" @click="refresh()" />
      <CreateContentType class="ms-1" @created="onCreated" @error="handleError" />
    </div>
    <div class="mb-3 row">
      <SearchInput class="col" :model-value="search" @update:model-value="setQuery('search', $event)" />
      <YesNoSelect
        class="col"
        id="invariant"
        label="contents.type.invariant.label"
        :model-value="isInvariant"
        @update:model-value="setQuery('invariant', $event?.toString() ?? '')"
      />
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
    <template v-if="contentTypes.length">
      <table class="table table-striped">
        <thead>
          <tr>
            <th scope="col">{{ t("contents.type.sort.options.UniqueName") }}</th>
            <th scope="col">{{ t("contents.type.sort.options.DisplayName") }}</th>
            <th scope="col">{{ t("contents.type.invariant.label") }}</th>
            <th scope="col">{{ t("contents.type.sort.options.UpdatedOn") }}</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="contentType in contentTypes" :key="contentType.id">
            <td>
              <RouterLink :to="{ name: 'ContentTypeEdit', params: { id: contentType.id } }"><EditIcon /> {{ contentType.uniqueName }}</RouterLink>
            </td>
            <td>
              <template v-if="contentType.displayName">{{ contentType.displayName }}</template>
              <span v-else class="text-muted">{{ "—" }}</span>
            </td>
            <td>
              <template v-if="contentType.isInvariant"><font-awesome-icon icon="fas fa-check" /> {{ t("yes") }}</template>
              <template v-else><font-awesome-icon icon="fas fa-times" /> {{ t("no") }}</template>
            </td>
            <td><StatusBlock :actor="contentType.updatedBy" :date="contentType.updatedOn" /></td>
          </tr>
        </tbody>
      </table>
      <AppPagination :count="count" :model-value="page" :total="total" @update:model-value="setQuery('page', $event.toString())" />
    </template>
    <p v-else>{{ t("contents.type.empty") }}</p>
  </main>
</template>
