<script setup lang="ts">
import type { SelectOption } from "logitar-vue3-ui";
import { arrayUtils, objectUtils } from "logitar-js";
import { computed, inject, ref, watch } from "vue";
import { parsingUtils } from "logitar-js";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import AppBreadcrumb from "@/components/shared/AppBreadcrumb.vue";
import AppPagination from "@/components/shared/AppPagination.vue";
import ContentTypeIcon from "@/components/contents/ContentTypeIcon.vue";
import ContentTypeSelect from "@/components/contents/ContentTypeSelect.vue";
import CountSelect from "@/components/shared/CountSelect.vue";
import CreateContent from "@/components/contents/CreateContent.vue";
import EditIcon from "@/components/shared/EditIcon.vue";
import LanguageIcon from "@/components/languages/LanguageIcon.vue";
import LanguageSelect from "@/components/languages/LanguageSelect.vue";
import RefreshButton from "@/components/shared/RefreshButton.vue";
import SearchInput from "@/components/shared/SearchInput.vue";
import SortSelect from "@/components/shared/SortSelect.vue";
import StatusBlock from "@/components/shared/StatusBlock.vue";
import type { Content, ContentLocale, ContentSort, SearchContentLocalesPayload } from "@/types/contents";
import type { SearchResults } from "@/types/search";
import { handleErrorKey } from "@/inject/App";
import { searchContentLocales } from "@/api/contents/items";
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
const locales = ref<ContentLocale[]>([]);
const timestamp = ref<number>(0);
const total = ref<number>(0);

const contentTypeId = computed<string>(() => route.query.type?.toString() ?? "");
const count = computed<number>(() => parseNumber(route.query.count?.toString()) || 10);
const isDescending = computed<boolean>(() => parseBoolean(route.query.isDescending?.toString()) ?? false);
const languageId = computed<string>(() => route.query.language?.toString() ?? "");
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
  const payload: SearchContentLocalesPayload = {
    contentTypeId: contentTypeId.value,
    ids: [],
    languageId: languageId.value,
    search: {
      terms: search.value
        .split(" ")
        .filter((term) => term.length > 0)
        .map((term) => ({ value: `%${term}%` })),
      operator: "And",
    },
    sort: sort.value ? [{ field: sort.value as ContentSort, isDescending: isDescending.value }] : [],
    skip: (page.value - 1) * count.value,
    limit: count.value,
  };
  isLoading.value = true;
  const now = Date.now();
  timestamp.value = now;
  try {
    const results: SearchResults<ContentLocale> = await searchContentLocales(payload);
    if (now === timestamp.value) {
      locales.value = results.items;
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
    case "language":
    case "search":
    case "type":
    case "count":
      query.page = "1";
      break;
  }
  router.replace({ ...route, query });
}

function onCreated(content: Content) {
  toasts.success("contents.item.created");
  router.push({ name: "ContentEdit", params: { id: content.id } });
}

watch(
  () => route,
  (route) => {
    if (route.name === "ContentList") {
      const { query } = route;
      if (!query.page || !query.count) {
        router.replace({
          ...route,
          query: isEmpty(query)
            ? {
                language: "",
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
    <h1>{{ t("contents.item.title") }}</h1>
    <AppBreadcrumb :current="t('contents.item.title')" />
    <div class="my-3">
      <RefreshButton class="me-1" :disabled="isLoading" :loading="isLoading" @click="refresh()" />
      <CreateContent class="ms-1" @created="onCreated" @error="handleError" />
    </div>
    <div class="mb-3 row">
      <ContentTypeSelect class="col" :model-value="contentTypeId" @update:model-value="setQuery('type', $event)" />
      <LanguageSelect class="col" :model-value="languageId" placeholder="contents.item.invariant" @update:model-value="setQuery('language', $event)" />
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
    <template v-if="locales.length">
      <table class="table table-striped">
        <thead>
          <tr>
            <th scope="col">{{ t("contents.item.name") }}</th>
            <th scope="col">{{ t("contents.type.select.label") }}</th>
            <th scope="col">{{ t("languages.select.label") }}</th>
            <th scope="col">{{ t("contents.item.sort.options.PublishedOn") }}</th>
            <th scope="col">{{ t("contents.item.sort.options.UpdatedOn") }}</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="locale in locales" :key="locale.content?.id">
            <td>
              <RouterLink v-if="locale.content" :to="{ name: 'ContentEdit', params: { id: locale.content.id } }">
                <EditIcon /> {{ locale.displayName ?? locale.uniqueName }}
                <template v-if="locale.displayName">
                  <br />
                  {{ locale.uniqueName }}
                </template>
              </RouterLink>
            </td>
            <td>
              <RouterLink v-if="locale.content?.contentType" :to="{ name: 'ContentTypeEdit', params: { id: locale.content.contentType.id } }" target="_blank">
                <ContentTypeIcon /> {{ locale.content.contentType.displayName ?? locale.content?.contentType.uniqueName }}
                <template v-if="locale.content.contentType.displayName">
                  <br />
                  {{ locale.content.contentType.uniqueName }}
                </template>
              </RouterLink>
            </td>
            <td>
              <RouterLink v-if="locale.language" :to="{ name: 'LanguageEdit', params: { id: locale.language.id } }" target="_blank">
                <LanguageIcon /> {{ locale.language.locale.code }}
                <template v-if="locale.language.id">
                  <br />
                  {{ locale.language.locale.displayName }}
                </template>
              </RouterLink>
              <span v-else class="text-muted">{{ t("contents.item.invariant") }}</span>
            </td>
            <td>
              <StatusBlock v-if="locale.publishedBy && locale.publishedOn" :actor="locale.publishedBy" :date="locale.publishedOn" />
              <span v-else class="text-muted">{{ t("contents.item.unpublished.label") }}</span>
            </td>
            <td><StatusBlock :actor="locale.updatedBy" :date="locale.updatedOn" /></td>
          </tr>
        </tbody>
      </table>
      <AppPagination :count="count" :model-value="page" :total="total" @update:model-value="setQuery('page', $event.toString())" />
    </template>
    <p v-else>{{ t("contents.item.empty") }}</p>
  </main>
</template>
