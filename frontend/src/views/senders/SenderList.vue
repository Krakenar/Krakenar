<script setup lang="ts">
import { TarBadge, TarButton, TarSelect, type SelectOption } from "logitar-vue3-ui";
import { arrayUtils, objectUtils } from "logitar-js";
import { computed, inject, ref, watch } from "vue";
import { parsingUtils } from "logitar-js";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import AppPagination from "@/components/shared/AppPagination.vue";
import CountSelect from "@/components/shared/CountSelect.vue";
import CreateSender from "@/components/senders/CreateSender.vue";
import SearchInput from "@/components/shared/SearchInput.vue";
import SenderKindSelect from "@/components/senders/SenderKindSelect.vue";
import SortSelect from "@/components/shared/SortSelect.vue";
import StatusBlock from "@/components/shared/StatusBlock.vue";
import type { SearchResults } from "@/types/search";
import type { Sender, SenderSort, SearchSendersPayload, SenderKind, SenderProvider } from "@/types/senders";
import { handleErrorKey } from "@/inject/App";
import { searchSenders } from "@/api/senders";
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
const senders = ref<Sender[]>([]);
const timestamp = ref<number>(0);
const total = ref<number>(0);

const count = computed<number>(() => parseNumber(route.query.count?.toString()) || 10);
const isDescending = computed<boolean>(() => parseBoolean(route.query.isDescending?.toString()) ?? false);
const kind = computed<string>(() => route.query.kind?.toString() ?? "");
const page = computed<number>(() => parseNumber(route.query.page?.toString()) || 1);
const provider = computed<string>(() => route.query.provider?.toString() ?? "");
const search = computed<string>(() => route.query.search?.toString() ?? "");
const sort = computed<string>(() => route.query.sort?.toString() ?? "");

const providerOptions = computed<SelectOption[]>(() =>
  orderBy(
    Object.entries(tm(rt("senders.provider.options"))).map(([value, text]) => ({ text, value }) as SelectOption),
    "text",
  ),
);
const sortOptions = computed<SelectOption[]>(() =>
  orderBy(
    Object.entries(tm(rt("senders.sort.options"))).map(([value, text]) => ({ text, value }) as SelectOption),
    "text",
  ),
);

async function refresh(): Promise<void> {
  const payload: SearchSendersPayload = {
    ids: [],
    kind: kind.value ? (kind.value as SenderKind) : undefined,
    provider: provider.value ? (provider.value as SenderProvider) : undefined,
    search: {
      terms: search.value
        .split(" ")
        .filter((term) => term.length > 0)
        .map((term) => ({ value: `%${term}%` })),
      operator: "And",
    },
    sort: sort.value ? [{ field: sort.value as SenderSort, isDescending: isDescending.value }] : [],
    skip: (page.value - 1) * count.value,
    limit: count.value,
  };
  isLoading.value = true;
  const now = Date.now();
  timestamp.value = now;
  try {
    const results: SearchResults<Sender> = await searchSenders(payload);
    if (now === timestamp.value) {
      senders.value = results.items;
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
    case "kind":
    case "provider":
    case "search":
    case "count":
      query.page = "1";
      break;
  }
  router.replace({ ...route, query });
}

function onCreated(sender: Sender) {
  toasts.success("senders.created");
  router.push({ name: "SenderEdit", params: { id: sender.id } });
}

watch(
  () => route,
  (route) => {
    if (route.name === "SenderList") {
      const { query } = route;
      if (!query.page || !query.count) {
        router.replace({
          ...route,
          query: isEmpty(query)
            ? {
                kind: "",
                provider: "",
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
    <h1>{{ t("senders.title") }}</h1>
    <div class="my-3">
      <TarButton
        class="me-1"
        :disabled="isLoading"
        icon="fas fa-rotate"
        :loading="isLoading"
        :status="t('loading')"
        :text="t('actions.refresh')"
        @click="refresh()"
      />
      <CreateSender class="ms-1" @created="onCreated" @error="handleError" />
    </div>
    <div class="mb-3 row">
      <SenderKindSelect class="col" :model-value="kind" @update:model-value="setQuery('kind', $event)" />
      <TarSelect
        class="col"
        floating
        id="provider"
        :label="t('senders.provider.label')"
        :model-value="provider"
        :options="providerOptions"
        :placeholder="t('senders.provider.placeholder')"
        @update:model-value="setQuery('provider', $event)"
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
    <template v-if="senders.length">
      <table class="table table-striped">
        <thead>
          <tr>
            <th scope="col">{{ t("senders.contact") }}</th>
            <th scope="col">{{ t("senders.sort.options.DisplayName") }}</th>
            <th scope="col">{{ t("senders.provider.label") }}</th>
            <th scope="col">{{ t("senders.sort.options.UpdatedOn") }}</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="sender in senders" :key="sender.id">
            <td>
              <RouterLink :to="{ name: 'SenderEdit', params: { id: sender.id } }">
                <template v-if="sender.email"><font-awesome-icon icon="fas fa-at" /> {{ sender.email.address }}</template>
                <template v-else-if="sender.phone"><font-awesome-icon icon="fas fa-phone" /> {{ sender.phone.e164Formatted }}</template>
                <template v-if="sender.isDefault">
                  <br />
                  <TarBadge><font-awesome-icon icon="fas fa-check" /> {{ t("senders.default") }}</TarBadge>
                </template>
              </RouterLink>
            </td>
            <td>
              <template v-if="sender.displayName">{{ sender.displayName }}</template>
              <span v-else class="text-muted">{{ "—" }}</span>
            </td>
            <td>{{ t(`senders.provider.options.${sender.provider}`) }}</td>
            <td><StatusBlock :actor="sender.updatedBy" :date="sender.updatedOn" /></td>
          </tr>
        </tbody>
      </table>
      <AppPagination :count="count" :model-value="page" :total="total" @update:model-value="setQuery('page', $event.toString())" />
    </template>
    <p v-else>{{ t("senders.empty") }}</p>
  </main>
</template>
