<script setup lang="ts">
import { TarButton, type SelectOption } from "logitar-vue3-ui";
import { arrayUtils, objectUtils } from "logitar-js";
import { computed, inject, ref, watch } from "vue";
import { parsingUtils } from "logitar-js";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import AppPagination from "@/components/shared/AppPagination.vue";
import CountSelect from "@/components/shared/CountSelect.vue";
import CreateUser from "@/components/users/CreateUser.vue";
import EnabledBadge from "@/components/users/EnabledBadge.vue";
import HasPasswordSelect from "@/components/users/HasPasswordSelect.vue";
import RoleSelect from "@/components/roles/RoleSelect.vue";
import SearchInput from "@/components/shared/SearchInput.vue";
import SortSelect from "@/components/shared/SortSelect.vue";
import StatusBlock from "@/components/shared/StatusBlock.vue";
import UserAvatar from "@/components/users/UserAvatar.vue";
import VerifiedBadge from "@/components/users/VerifiedBadge.vue";
import YesNoSelect from "@/components/shared/YesNoSelect.vue";
import type { SearchResults } from "@/types/search";
import type { User, UserSort, SearchUsersPayload } from "@/types/users";
import { handleErrorKey } from "@/inject/App";
import { searchUsers } from "@/api/users";
import { useToastStore } from "@/stores/toast";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const toasts = useToastStore();
const { d, rt, t, tm } = useI18n();
const { isEmpty } = objectUtils;
const { orderBy } = arrayUtils;
const { parseBoolean, parseNumber } = parsingUtils;

const isLoading = ref<boolean>(false);
const timestamp = ref<number>(0);
const total = ref<number>(0);
const users = ref<User[]>([]);

const count = computed<number>(() => parseNumber(route.query.count?.toString()) || 10);
const hasAuthenticated = computed<boolean | undefined>(() => parseBoolean(route.query.authenticated?.toString()));
const hasPassword = computed<boolean | undefined>(() => parseBoolean(route.query.password?.toString()));
const isConfirmed = computed<boolean | undefined>(() => parseBoolean(route.query.confirmed?.toString()));
const isDescending = computed<boolean>(() => parseBoolean(route.query.isDescending?.toString()) ?? false);
const isDisabled = computed<boolean | undefined>(() => parseBoolean(route.query.disabled?.toString()));
const page = computed<number>(() => parseNumber(route.query.page?.toString()) || 1);
const roleId = computed<string>(() => route.query.role?.toString() ?? "");
const search = computed<string>(() => route.query.search?.toString() ?? "");
const sort = computed<string>(() => route.query.sort?.toString() ?? "");

const sortOptions = computed<SelectOption[]>(() =>
  orderBy(
    Object.entries(tm(rt("users.sort.options"))).map(([value, text]) => ({ text, value }) as SelectOption),
    "text",
  ),
);

async function refresh(): Promise<void> {
  const payload: SearchUsersPayload = {
    hasAuthenticated: hasAuthenticated.value,
    hasPassword: hasPassword.value,
    ids: [],
    isConfirmed: isConfirmed.value,
    isDisabled: isDisabled.value,
    roleId: roleId.value,
    search: {
      terms: search.value
        .split(" ")
        .filter((term) => term.length > 0)
        .map((term) => ({ value: `%${term}%` })),
      operator: "And",
    },
    sort: sort.value ? [{ field: sort.value as UserSort, isDescending: isDescending.value }] : [],
    skip: (page.value - 1) * count.value,
    limit: count.value,
  };
  isLoading.value = true;
  const now = Date.now();
  timestamp.value = now;
  try {
    const results: SearchResults<User> = await searchUsers(payload);
    if (now === timestamp.value) {
      users.value = results.items;
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
    case "confirmed":
    case "disabled":
    case "password":
    case "role":
    case "search":
    case "count":
      query.page = "1";
      break;
  }
  router.replace({ ...route, query });
}

function onCreated(user: User) {
  toasts.success("users.created");
  router.push({ name: "UserEdit", params: { id: user.id } });
}

watch(
  () => route,
  (route) => {
    if (route.name === "UserList") {
      const { query } = route;
      if (!query.page || !query.count) {
        router.replace({
          ...route,
          query: isEmpty(query)
            ? {
                authenticated: "",
                confirmed: "",
                disabled: "",
                password: "",
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
  <main class="container-fluid">
    <h1>{{ t("users.title") }}</h1>
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
      <CreateUser class="ms-1" @created="onCreated" @error="handleError" />
    </div>
    <div class="mb-3 row">
      <HasPasswordSelect class="col" :model-value="hasPassword" @update:model-value="setQuery('password', $event?.toString() ?? '')" />
      <YesNoSelect
        class="col"
        id="disabled"
        label="users.disabled.label"
        :model-value="isDisabled"
        @update:model-value="setQuery('disabled', $event?.toString() ?? '')"
      />
      <YesNoSelect
        class="col"
        id="confirmed"
        label="users.confirmed"
        :model-value="isConfirmed"
        @update:model-value="setQuery('confirmed', $event?.toString() ?? '')"
      />
      <YesNoSelect
        class="col"
        id="authenticated"
        label="users.authenticated"
        :model-value="hasAuthenticated"
        @update:model-value="setQuery('authenticated', $event?.toString() ?? '')"
      />
    </div>
    <div class="mb-3 row">
      <SearchInput class="col" :model-value="search" @update:model-value="setQuery('search', $event)" />
      <RoleSelect class="col" :model-value="roleId" @update:model-value="setQuery('role', $event?.toString() ?? '')" />
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
    <template v-if="users.length">
      <table class="table table-striped">
        <thead>
          <tr>
            <th scope="col">{{ t("users.select.label") }}</th>
            <th scope="col">{{ t("users.sort.options.PasswordChangedOn") }}</th>
            <th scope="col">{{ t("users.sort.options.DisabledOn") }}</th>
            <th scope="col">{{ t("users.contact") }}</th>
            <th scope="col">{{ t("users.sort.options.Birthdate") }}</th>
            <th scope="col">{{ t("users.sort.options.AuthenticatedOn") }}</th>
            <th scope="col">{{ t("users.sort.options.UpdatedOn") }}</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="user in users" :key="user.id">
            <td><UserAvatar :user="user" /></td>
            <td>
              <StatusBlock v-if="user.passwordChangedBy && user.passwordChangedOn" :actor="user.passwordChangedBy" :date="user.passwordChangedOn" />
              <span class="ms-1 text-muted" v-else>{{ "—" }}</span>
            </td>
            <td>
              <StatusBlock v-if="user.disabledBy && user.disabledOn" :actor="user.disabledBy" :date="user.disabledOn" />
              <EnabledBadge v-else />
            </td>
            <td>
              <font-awesome-icon class="me-1" icon="fas fa-at" />
              <template v-if="user.email">
                {{ user.email.address }}
                <VerifiedBadge v-if="user.email.isVerified" class="ms-1" text="users.email.verified.label" />
              </template>
              <span class="ms-1 text-muted" v-else>{{ "—" }}</span>
              <br />
              <font-awesome-icon class="me-1" icon="fas fa-phone" />
              <template v-if="user.phone">
                {{ user.phone.e164Formatted }}
                <VerifiedBadge v-if="user.phone.isVerified" class="ms-1" text="users.phone.verified.label" />
              </template>
              <span class="ms-1 text-muted" v-else>{{ "—" }}</span>
            </td>
            <td>
              <template v-if="user.birthdate">{{ d(user.birthdate, "medium") }}</template>
              <span v-else class="text-muted">{{ "—" }}</span>
            </td>
            <td>
              <template v-if="user.authenticatedOn">{{ d(user.authenticatedOn, "medium") }}</template>
              <span v-else class="text-muted">{{ "—" }}</span>
            </td>
            <td><StatusBlock :actor="user.updatedBy" :date="user.updatedOn" /></td>
          </tr>
        </tbody>
      </table>
      <AppPagination :count="count" :model-value="page" :total="total" @update:model-value="setQuery('page', $event.toString())" />
    </template>
    <p v-else>{{ t("users.empty") }}</p>
  </main>
</template>
