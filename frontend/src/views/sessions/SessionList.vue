<script setup lang="ts">
import type { SelectOption } from "logitar-vue3-ui";
import { arrayUtils, objectUtils } from "logitar-js";
import { computed, inject, ref, watch } from "vue";
import { parsingUtils } from "logitar-js";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import ActiveBadge from "@/components/sessions/ActiveBadge.vue";
import AppBreadcrumb from "@/components/shared/AppBreadcrumb.vue";
import AppPagination from "@/components/shared/AppPagination.vue";
import CountSelect from "@/components/shared/CountSelect.vue";
import EditIcon from "@/components/shared/EditIcon.vue";
import PersistentBadge from "@/components/sessions/PersistentBadge.vue";
import RefreshButton from "@/components/shared/RefreshButton.vue";
import SearchInput from "@/components/shared/SearchInput.vue";
import SortSelect from "@/components/shared/SortSelect.vue";
import StatusBlock from "@/components/shared/StatusBlock.vue";
import UserAvatar from "@/components/users/UserAvatar.vue";
import UserSelect from "@/components/users/UserSelect.vue";
import YesNoSelect from "@/components/shared/YesNoSelect.vue";
import type { SearchResults } from "@/types/search";
import type { Session, SessionSort, SearchSessionsPayload } from "@/types/sessions";
import { handleErrorKey } from "@/inject/App";
import { searchSessions } from "@/api/sessions";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const { d, rt, t, tm } = useI18n();
const { isEmpty } = objectUtils;
const { orderBy } = arrayUtils;
const { parseBoolean, parseNumber } = parsingUtils;

const isLoading = ref<boolean>(false);
const sessions = ref<Session[]>([]);
const timestamp = ref<number>(0);
const total = ref<number>(0);

const count = computed<number>(() => parseNumber(route.query.count?.toString()) || 10);
const isActive = computed<boolean | undefined>(() => parseBoolean(route.query.active?.toString()));
const isDescending = computed<boolean>(() => parseBoolean(route.query.isDescending?.toString()) ?? false);
const isPersistent = computed<boolean | undefined>(() => parseBoolean(route.query.persistent?.toString()));
const page = computed<number>(() => parseNumber(route.query.page?.toString()) || 1);
const search = computed<string>(() => route.query.search?.toString() ?? "");
const sort = computed<string>(() => route.query.sort?.toString() ?? "");
const userId = computed<string>(() => route.query.user?.toString() ?? "");

const sortOptions = computed<SelectOption[]>(() =>
  orderBy(
    Object.entries(tm(rt("sessions.sort.options"))).map(([value, text]) => ({ text, value }) as SelectOption),
    "text",
  ),
);

async function refresh(): Promise<void> {
  const payload: SearchSessionsPayload = {
    ids: [],
    isActive: isActive.value,
    isPersistent: isPersistent.value,
    search: {
      terms: search.value
        .split(" ")
        .filter((term) => term.length > 0)
        .map((term) => ({ value: `%${term}%` })),
      operator: "And",
    },
    userId: userId.value,
    sort: sort.value ? [{ field: sort.value as SessionSort, isDescending: isDescending.value }] : [],
    skip: (page.value - 1) * count.value,
    limit: count.value,
  };
  isLoading.value = true;
  const now = Date.now();
  timestamp.value = now;
  try {
    const results: SearchResults<Session> = await searchSessions(payload);
    if (now === timestamp.value) {
      sessions.value = results.items;
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
    case "active":
    case "persistent":
    case "search":
    case "user":
    case "count":
      query.page = "1";
      break;
  }
  router.replace({ ...route, query });
}

watch(
  () => route,
  (route) => {
    if (route.name === "SessionList") {
      const { query } = route;
      if (!query.page || !query.count) {
        router.replace({
          ...route,
          query: isEmpty(query)
            ? {
                active: "",
                persistent: "",
                search: "",
                user: "",
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
    <h1>{{ t("sessions.title.list") }}</h1>
    <AppBreadcrumb :current="t('sessions.title.list')" />
    <div class="my-3">
      <RefreshButton class="me-1" :disabled="isLoading" :loading="isLoading" @click="refresh()" />
    </div>
    <div class="mb-3 row">
      <UserSelect class="col" :model-value="userId" @error="handleError" @update:model-value="setQuery('user', $event)" />
      <YesNoSelect
        class="col"
        id="active"
        label="sessions.active.label"
        :model-value="isActive"
        @update:model-value="setQuery('active', $event?.toString() ?? '')"
      />
      <YesNoSelect
        class="col"
        id="persistent"
        label="sessions.persistent"
        :model-value="isPersistent"
        @update:model-value="setQuery('persistent', $event?.toString() ?? '')"
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
    <template v-if="sessions.length">
      <table class="table table-striped">
        <thead>
          <tr>
            <th scope="col">{{ t("sessions.sort.options.CreatedOn") }}</th>
            <th scope="col">{{ t("users.select.label") }}</th>
            <th scope="col">{{ t("sessions.sort.options.SignedOutOn") }}</th>
            <th scope="col">{{ t("sessions.sort.options.UpdatedOn") }}</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="session in sessions" :key="session.id">
            <td>
              <RouterLink :to="{ name: 'SessionEdit', params: { id: session.id } }"><EditIcon /> {{ d(session.createdOn, "medium") }}</RouterLink>
              <template v-if="session.isPersistent">
                <br />
                <PersistentBadge />
              </template>
            </td>
            <td>
              <UserAvatar target="_blank" :user="session.user" />
            </td>
            <td>
              <StatusBlock v-if="session.signedOutBy && session.signedOutOn" :actor="session.signedOutBy" :date="session.signedOutOn" />
              <ActiveBadge v-else />
            </td>
            <td><StatusBlock :actor="session.updatedBy" :date="session.updatedOn" /></td>
          </tr>
        </tbody>
      </table>
      <AppPagination :count="count" :model-value="page" :total="total" @update:model-value="setQuery('page', $event.toString())" />
    </template>
    <p v-else>{{ t("sessions.empty") }}</p>
  </main>
</template>
