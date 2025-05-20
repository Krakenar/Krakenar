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
import DefaultBadge from "@/components/senders/DefaultBadge.vue";
import DemoBadge from "@/components/messages/DemoBadge.vue";
import RefreshButton from "@/components/shared/RefreshButton.vue";
import SearchInput from "@/components/shared/SearchInput.vue";
import SenderIcon from "@/components/senders/SenderIcon.vue";
import SortSelect from "@/components/shared/SortSelect.vue";
import StatusBadge from "@/components/messages/StatusBadge.vue";
import StatusBlock from "@/components/shared/StatusBlock.vue";
import StatusSelect from "@/components/messages/StatusSelect.vue";
import TemplateIcon from "@/components/templates/TemplateIcon.vue";
import TemplateSelect from "@/components/templates/TemplateSelect.vue";
import ViewIcon from "@/components/shared/ViewIcon.vue";
import YesNoSelect from "@/components/shared/YesNoSelect.vue";
import type { Message, MessageSort, MessageStatus, SearchMessagesPayload } from "@/types/messages";
import type { SearchResults } from "@/types/search";
import { formatSender, formatTemplate } from "@/helpers/format";
import { handleErrorKey } from "@/inject/App";
import { searchMessages } from "@/api/messages";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const { isEmpty } = objectUtils;
const { orderBy } = arrayUtils;
const { parseBoolean, parseNumber } = parsingUtils;
const { rt, t, tm } = useI18n();

const isLoading = ref<boolean>(false);
const messages = ref<Message[]>([]);
const timestamp = ref<number>(0);
const total = ref<number>(0);

const count = computed<number>(() => parseNumber(route.query.count?.toString()) || 10);
const isDemo = computed<boolean | undefined>(() => parseBoolean(route.query.demo?.toString()));
const isDescending = computed<boolean>(() => parseBoolean(route.query.isDescending?.toString()) ?? false);
const page = computed<number>(() => parseNumber(route.query.page?.toString()) || 1);
const search = computed<string>(() => route.query.search?.toString() ?? "");
const sort = computed<string>(() => route.query.sort?.toString() ?? "");
const status = computed<string>(() => route.query.status?.toString() ?? "");
const template = computed<string>(() => route.query.template?.toString() ?? "");

const sortOptions = computed<SelectOption[]>(() =>
  orderBy(
    Object.entries(tm(rt("messages.sort.options"))).map(([value, text]) => ({ text, value }) as SelectOption),
    "text",
  ),
);

async function refresh(): Promise<void> {
  const payload: SearchMessagesPayload = {
    ids: [],
    isDemo: isDemo.value,
    search: {
      terms: search.value
        .split(" ")
        .filter((term) => term.length > 0)
        .map((term) => ({ value: `%${term}%` })),
      operator: "And",
    },
    status: status.value ? (status.value as MessageStatus) : undefined,
    templateId: template.value,
    sort: sort.value ? [{ field: sort.value as MessageSort, isDescending: isDescending.value }] : [],
    skip: (page.value - 1) * count.value,
    limit: count.value,
  };
  isLoading.value = true;
  const now = Date.now();
  timestamp.value = now;
  try {
    const results: SearchResults<Message> = await searchMessages(payload);
    if (now === timestamp.value) {
      messages.value = results.items;
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
    case "demo":
    case "search":
    case "status":
    case "template":
    case "count":
      query.page = "1";
      break;
  }
  router.replace({ ...route, query });
}

watch(
  () => route,
  (route) => {
    if (route.name === "MessageList") {
      const { query } = route;
      if (!query.page || !query.count) {
        router.replace({
          ...route,
          query: isEmpty(query)
            ? {
                demo: "",
                search: "",
                status: "",
                template: "",
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
  <main>
    <div class="container">
      <h1>{{ t("messages.title") }}</h1>
      <AppBreadcrumb :current="t('messages.title')" />
      <div class="my-3">
        <RefreshButton :disabled="isLoading" :loading="isLoading" @click="refresh()" />
      </div>
      <div class="mb-3 row">
        <YesNoSelect class="col" id="demo" label="messages.demo.label" :model-value="isDemo" @update:model-value="setQuery('demo', $event?.toString() ?? '')" />
        <TemplateSelect class="col" :model-value="template" @update:model-value="setQuery('template', $event)" />
        <StatusSelect class="col" :model-value="status" @update:model-value="setQuery('status', $event)" />
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
    </div>
    <div v-if="messages.length" class="container-fluid">
      <table class="table table-striped">
        <thead>
          <tr>
            <th scope="col">{{ t("messages.sort.options.Subject") }}</th>
            <th scope="col">{{ t("messages.sort.options.RecipientCount") }}</th>
            <th scope="col">{{ t("senders.select.label") }}</th>
            <th scope="col">{{ t("templates.select.label") }}</th>
            <th scope="col">{{ t("messages.status.label") }}</th>
            <th scope="col">{{ t("messages.sort.options.UpdatedOn") }}</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="message in messages" :key="message.id">
            <td>
              <RouterLink :to="{ name: 'MessageView', params: { id: message.id } }">
                <ViewIcon /> {{ message.subject }}
                <template v-if="message.isDemo">
                  <br />
                  <DemoBadge />
                </template>
              </RouterLink>
            </td>
            <td>{{ message.recipientCount }}</td>
            <td>
              <RouterLink v-if="message.sender.version > 0" :to="{ name: 'SenderEdit', params: { id: message.sender.id } }" target="_blank">
                <SenderIcon :sender="message.sender" /> {{ formatSender(message.sender) }}
                <template v-if="message.sender.isDefault">
                  <br />
                  <DefaultBadge />
                </template>
              </RouterLink>
              <span v-else class="text-muted"><SenderIcon :sender="message.sender" /> {{ formatSender(message.sender) }}</span>
            </td>
            <td>
              <RouterLink v-if="message.template.version > 0" :to="{ name: 'TemplateEdit', params: { id: message.template.id } }" target="_blank">
                <TemplateIcon /> {{ formatTemplate(message.template) }}
              </RouterLink>
              <span v-else class="text-muted"><TemplateIcon /> {{ formatTemplate(message.template) }}</span>
            </td>
            <td><StatusBadge :message="message" /></td>
            <td><StatusBlock :actor="message.updatedBy" :date="message.updatedOn" /></td>
          </tr>
        </tbody>
      </table>
      <AppPagination :count="count" :model-value="page" :total="total" @update:model-value="setQuery('page', $event.toString())" />
    </div>
    <div v-else class="container">
      <p>{{ t("messages.empty") }}</p>
    </div>
  </main>
</template>
