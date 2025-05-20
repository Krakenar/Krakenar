<script setup lang="ts">
import { TarTab, TarTabs } from "logitar-vue3-ui";
import { computed, inject, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import AppBreadcrumb from "@/components/shared/AppBreadcrumb.vue";
import DeleteTemplate from "@/components/templates/DeleteTemplate.vue";
import SendDemo from "@/components/messages/SendDemo.vue";
import StatusDetail from "@/components/shared/StatusDetail.vue";
import TemplateContents from "@/components/templates/TemplateContents.vue";
import TemplateGeneral from "@/components/templates/TemplateGeneral.vue";
import type { Breadcrumb } from "@/types/breadcrumb";
import type { Configuration } from "@/types/configuration";
import type { Template } from "@/types/templates";
import { StatusCodes, type ApiFailure } from "@/types/api";
import { formatTemplate } from "@/helpers/format";
import { handleErrorKey } from "@/inject/App";
import { readConfiguration } from "@/api/configuration";
import { readTemplate } from "@/api/templates";
import { useToastStore } from "@/stores/toast";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const toasts = useToastStore();
const { t } = useI18n();

const configuration = ref<Configuration>();
const template = ref<Template>();

const breadcrumb = computed<Breadcrumb[]>(() => [{ route: { name: "TemplateList" }, text: t("templates.title") }]);
const title = computed<string>(() => (template.value ? formatTemplate(template.value) : ""));

function setMetadata(updated: Template): void {
  if (template.value) {
    template.value.version = updated.version;
    template.value.updatedBy = updated.updatedBy;
    template.value.updatedOn = updated.updatedOn;
  }
}

function onDeleted(): void {
  toasts.success("templates.deleted");
  router.push({ name: "TemplateList" });
}

function onContentsUpdated(updated: Template): void {
  if (template.value) {
    setMetadata(updated);
    template.value.subject = updated.subject;
    template.value.content = { ...updated.content };
  }
  toasts.success("templates.updated");
}

function onGeneralUpdated(updated: Template): void {
  if (template.value) {
    setMetadata(updated);
    template.value.uniqueName = updated.uniqueName;
    template.value.displayName = updated.displayName;
    template.value.description = updated.description;
  }
  toasts.success("templates.updated");
}

onMounted(async () => {
  try {
    const id = route.params.id as string;
    template.value = await readTemplate(id);
    if (!template.value.realm) {
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
    <template v-if="template">
      <h1>{{ title }}</h1>
      <AppBreadcrumb :current="title" :parent="breadcrumb" />
      <StatusDetail :aggregate="template" />
      <div class="mb-3">
        <DeleteTemplate :template="template" @deleted="onDeleted" @error="handleError" />
      </div>
      <TarTabs>
        <TarTab active id="general" :title="t('general')">
          <TemplateGeneral :configuration="configuration" :template="template" @error="handleError" @updated="onGeneralUpdated" />
        </TarTab>
        <TarTab id="contents" :title="t('templates.content')">
          <TemplateContents :template="template" @error="handleError" @updated="onContentsUpdated" />
        </TarTab>
        <TarTab id="demo" :title="t('messages.demo.label')">
          <SendDemo :template="template" @error="handleError" />
        </TarTab>
      </TarTabs>
    </template>
  </main>
</template>
