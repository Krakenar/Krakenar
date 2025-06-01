<script setup lang="ts">
import { TarTab, TarTabs } from "logitar-vue3-ui";
import { computed, inject, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import AppBreadcrumb from "@/components/shared/AppBreadcrumb.vue";
import ContentTypeGeneral from "@/components/contents/ContentTypeGeneral.vue";
import DeleteContentType from "@/components/contents/DeleteContentType.vue";
import FieldDefinitionList from "@/components/fields/FieldDefinitionList.vue";
import StatusDetail from "@/components/shared/StatusDetail.vue";
import type { Breadcrumb } from "@/types/breadcrumb";
import type { ContentType } from "@/types/contents";
import { StatusCodes, type ApiFailure } from "@/types/api";
import { formatContentType } from "@/helpers/format";
import { handleErrorKey } from "@/inject/App";
import { readContentType } from "@/api/contents/types";
import { useToastStore } from "@/stores/toast";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const toasts = useToastStore();
const { t } = useI18n();

const contentType = ref<ContentType>();

const breadcrumb = computed<Breadcrumb[]>(() => [{ route: { name: "ContentTypeList" }, text: t("contents.type.title") }]);
const title = computed<string>(() => (contentType.value ? formatContentType(contentType.value) : ""));

function setMetadata(updated: ContentType): void {
  if (contentType.value) {
    contentType.value.version = updated.version;
    contentType.value.updatedBy = updated.updatedBy;
    contentType.value.updatedOn = updated.updatedOn;
  }
}

function onDeleted(): void {
  toasts.success("contents.type.deleted");
  router.push({ name: "ContentTypeList" });
}

function onGeneralUpdated(updated: ContentType): void {
  if (contentType.value) {
    setMetadata(updated);
    contentType.value.isInvariant = updated.isInvariant;
    contentType.value.uniqueName = updated.uniqueName;
    contentType.value.displayName = updated.displayName;
    contentType.value.description = updated.description;
  }
  toasts.success("contents.type.updated");
}

function onFieldCreated(updated: ContentType): void {
  if (contentType.value) {
    setMetadata(updated);
    contentType.value.fields = [...updated.fields];
  }
  toasts.success("fields.definition.created");
}
function onFieldDeleted(updated: ContentType): void {
  if (contentType.value) {
    setMetadata(updated);
    contentType.value.fields = [...updated.fields];
  }
  toasts.success("fields.definition.deleted");
}
function onFieldReordered(updated: ContentType): void {
  if (contentType.value) {
    setMetadata(updated);
    contentType.value.fields = [...updated.fields];
  }
  toasts.success("fields.definition.reordered");
}
function onFieldUpdated(updated: ContentType): void {
  if (contentType.value) {
    setMetadata(updated);
    contentType.value.fields = [...updated.fields];
  }
  toasts.success("fields.definition.updated");
}

onMounted(async () => {
  try {
    const id = route.params.id as string;
    contentType.value = await readContentType(id);
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
    <template v-if="contentType">
      <h1>{{ title }}</h1>
      <AppBreadcrumb :current="title" :parent="breadcrumb" />
      <StatusDetail :aggregate="contentType" />
      <div class="mb-3">
        <DeleteContentType :contentType="contentType" @deleted="onDeleted" @error="handleError" />
      </div>
      <TarTabs>
        <TarTab active id="general" :title="t('general')">
          <ContentTypeGeneral :content-type="contentType" @error="handleError" @updated="onGeneralUpdated" />
        </TarTab>
        <TarTab id="fields" :title="t('fields.definition.title')">
          <FieldDefinitionList
            :content-type="contentType"
            @error="handleError"
            @created="onFieldCreated"
            @deleted="onFieldDeleted"
            @reordered="onFieldReordered"
            @updated="onFieldUpdated"
          />
        </TarTab>
      </TarTabs>
    </template>
  </main>
</template>
