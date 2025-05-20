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
import CreateTemplate from "@/components/templates/CreateTemplate.vue";
import EditIcon from "@/components/shared/EditIcon.vue";
import MediaTypeSelect from "@/components/contents/MediaTypeSelect.vue";
import RefreshButton from "@/components/shared/RefreshButton.vue";
import SearchInput from "@/components/shared/SearchInput.vue";
import SortSelect from "@/components/shared/SortSelect.vue";
import StatusBlock from "@/components/shared/StatusBlock.vue";
import type { MediaType } from "@/types/contents";
import type { SearchResults } from "@/types/search";
import type { Template, TemplateSort, SearchTemplatesPayload } from "@/types/templates";
import { handleErrorKey } from "@/inject/App";
import { searchTemplates } from "@/api/templates";
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
const templates = ref<Template[]>([]);
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
    Object.entries(tm(rt("templates.sort.options"))).map(([value, text]) => ({ text, value }) as SelectOption),
    "text",
  ),
);

async function refresh(): Promise<void> {
  const payload: SearchTemplatesPayload = {
    ids: [],
    search: {
      terms: search.value
        .split(" ")
        .filter((term) => term.length > 0)
        .map((term) => ({ value: `%${term}%` })),
      operator: "And",
    },
    contentType: type.value ? (type.value as MediaType) : undefined,
    sort: sort.value ? [{ field: sort.value as TemplateSort, isDescending: isDescending.value }] : [],
    skip: (page.value - 1) * count.value,
    limit: count.value,
  };
  isLoading.value = true;
  const now = Date.now();
  timestamp.value = now;
  try {
    const results: SearchResults<Template> = await searchTemplates(payload);
    if (now === timestamp.value) {
      templates.value = results.items;
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

function onCreated(template: Template) {
  toasts.success("templates.created");
  router.push({ name: "TemplateEdit", params: { id: template.id } });
}

watch(
  () => route,
  (route) => {
    if (route.name === "TemplateList") {
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
    <h1>{{ t("templates.title") }}</h1>
    <AppBreadcrumb :current="t('templates.title')" />
    <div class="my-3">
      <RefreshButton class="me-1" :disabled="isLoading" :loading="isLoading" @click="refresh()" />
      <CreateTemplate class="ms-1" @created="onCreated" @error="handleError" />
    </div>
    <div class="mb-3 row">
      <SearchInput class="col" :model-value="search" @update:model-value="setQuery('search', $event)" />
      <MediaTypeSelect class="col" :model-value="type" @update:model-value="setQuery('type', $event)" />
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
    <template v-if="templates.length">
      <table class="table table-striped">
        <thead>
          <tr>
            <th scope="col">{{ t("templates.sort.options.UniqueName") }}</th>
            <th scope="col">{{ t("templates.sort.options.DisplayName") }}</th>
            <th scope="col">{{ t("templates.sort.options.Subject") }}</th>
            <th scope="col">{{ t("contents.media.label") }}</th>
            <th scope="col">{{ t("templates.sort.options.UpdatedOn") }}</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="template in templates" :key="template.id">
            <td>
              <RouterLink :to="{ name: 'TemplateEdit', params: { id: template.id } }"><EditIcon /> {{ template.uniqueName }}</RouterLink>
            </td>
            <td>
              <template v-if="template.displayName">{{ template.displayName }}</template>
              <span v-else class="text-muted">{{ "—" }}</span>
            </td>
            <td>{{ template.subject }}</td>
            <td>{{ t(`contents.media.options.${template.content.type}`) }}</td>
            <td><StatusBlock :actor="template.updatedBy" :date="template.updatedOn" /></td>
          </tr>
        </tbody>
      </table>
      <AppPagination :count="count" :model-value="page" :total="total" @update:model-value="setQuery('page', $event.toString())" />
    </template>
    <p v-else>{{ t("templates.empty") }}</p>
  </main>
</template>
