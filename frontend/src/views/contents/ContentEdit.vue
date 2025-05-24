<script setup lang="ts">
import { computed, inject, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import AppBreadcrumb from "@/components/shared/AppBreadcrumb.vue";
import ContentTypeInput from "@/components/contents/ContentTypeInput.vue";
import DeleteContent from "@/components/contents/DeleteContent.vue";
import PublishButton from "@/components/contents/PublishButton.vue";
import StatusDetail from "@/components/shared/StatusDetail.vue";
import UnpublishButton from "@/components/contents/UnpublishButton.vue";
import type { Breadcrumb } from "@/types/breadcrumb";
import type { Configuration } from "@/types/configuration";
import type { Content } from "@/types/contents";
import { StatusCodes, type ApiFailure } from "@/types/api";
import { handleErrorKey } from "@/inject/App";
import { readConfiguration } from "@/api/configuration";
import { readContent } from "@/api/contents/items";
import { useToastStore } from "@/stores/toast";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const toasts = useToastStore();
const { t } = useI18n();

const configuration = ref<Configuration>();
const content = ref<Content>();

const breadcrumb = computed<Breadcrumb[]>(() => [{ route: { name: "ContentList" }, text: t("contents.item.title") }]);
const title = computed<string>(() => (content.value ? (content.value.invariant.displayName ?? content.value.invariant.uniqueName) : ""));

function onDeleted(): void {
  toasts.success("contents.item.deleted");
  router.push({ name: "ContentList" });
}

function onPublished(value: Content): void {
  content.value = value;
  toasts.success("contents.item.published");
}
function onUnpublished(value: Content): void {
  content.value = value;
  toasts.success("contents.item.unpublished.success");
}

onMounted(async () => {
  try {
    const id = route.params.id as string;
    content.value = await readContent(id);
    if (!content.value.contentType.realm) {
      configuration.value = await readConfiguration();
    }
  } catch (e: unknown) {
    const { status } = e as ApiFailure;
    if (status === StatusCodes.NotFound) {
      router.push("/not-found");
    } else {
      handleError(e);
    }
  }
});
</script>

<template>
  <main class="container">
    <template v-if="content">
      <h1>{{ title }}</h1>
      <AppBreadcrumb :current="title" :parent="breadcrumb" />
      <StatusDetail :aggregate="content" />
      <div class="mb-3">
        <DeleteContent class="me-1" :content="content" @deleted="onDeleted" @error="handleError" />
        <PublishButton class="mx-1" :content="content" @error="handleError" @published="onPublished" />
        <UnpublishButton class="ms-1" :content="content" @error="handleError" @unpublished="onUnpublished" />
      </div>
      <div class="mb-3 row">
        <ContentTypeInput class="col" :content-type="content.contentType" />
      </div>
      <!-- TODO(fpion): Invariant without Tabs if no locale -->
      <!-- TODO(fpion): locale tabs with add button, LanguageSelect with exclude -->
      <!-- TODO(fpion): tabs contain StatusDetail with Published, Delete and Publish buttons (except Invariant), UniqueName, DisplayName, Description, FieldValues -->
    </template>
  </main>
</template>
