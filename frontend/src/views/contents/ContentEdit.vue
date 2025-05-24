<script setup lang="ts">
import { computed, inject, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import AppBreadcrumb from "@/components/shared/AppBreadcrumb.vue";
import ContentLocaleEdit from "@/components/contents/ContentLocaleEdit.vue";
import ContentTypeInput from "@/components/contents/ContentTypeInput.vue";
import DeleteContent from "@/components/contents/DeleteContent.vue";
import PublishButton from "@/components/contents/PublishButton.vue";
import PublishedInfo from "@/components/contents/PublishedInfo.vue";
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
const isTypeInvariant = computed<boolean>(() => content.value?.contentType.isInvariant ?? false);
const title = computed<string>(() => (content.value ? (content.value.invariant.displayName ?? content.value.invariant.uniqueName) : ""));

function onDeleted(): void {
  toasts.success("contents.item.deleted");
  router.push({ name: "ContentList" });
}

function onPublished(value: Content): void {
  content.value = value;
  toasts.success("contents.item.published.success");
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
      <StatusDetail :aggregate="content">
        <template v-if="isTypeInvariant">
          <br />
          <PublishedInfo :locale="content.invariant" />
        </template>
      </StatusDetail>
      <div class="mb-3">
        <DeleteContent class="me-1" :content="content" @deleted="onDeleted" @error="handleError" />
        <PublishButton class="mx-1" :content="content" @error="handleError" @published="onPublished" />
        <UnpublishButton class="ms-1" :content="content" @error="handleError" @unpublished="onUnpublished" />
      </div>
      <div class="row">
        <ContentTypeInput class="col" :content-type="content.contentType" />
      </div>
      <ContentLocaleEdit
        v-if="isTypeInvariant"
        :configuration="configuration"
        :content-type="content.contentType"
        :locale="content.invariant"
        @error="handleError"
      />
      <!-- TODO(fpion): locale tabs with add button, LanguageSelect with exclude -->
    </template>
  </main>
</template>
