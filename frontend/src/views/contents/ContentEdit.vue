<script setup lang="ts">
import { TarButton, TarTab, TarTabs } from "logitar-vue3-ui";
import { computed, inject, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import AppBreadcrumb from "@/components/shared/AppBreadcrumb.vue";
import ContentLocaleEdit from "@/components/contents/ContentLocaleEdit.vue";
import ContentTypeInput from "@/components/contents/ContentTypeInput.vue";
import DeleteContent from "@/components/contents/DeleteContent.vue";
import LanguageSelect from "@/components/languages/LanguageSelect.vue";
import PublishButton from "@/components/contents/PublishButton.vue";
import PublishedInfo from "@/components/contents/PublishedInfo.vue";
import StatusDetail from "@/components/shared/StatusDetail.vue";
import UnpublishButton from "@/components/contents/UnpublishButton.vue";
import type { Actor } from "@/types/actor";
import type { Breadcrumb } from "@/types/breadcrumb";
import type { Configuration } from "@/types/configuration";
import type { Content, ContentLocale } from "@/types/contents";
import type { Language } from "@/types/languages";
import { StatusCodes, type ApiFailure } from "@/types/api";
import { handleErrorKey } from "@/inject/App";
import { readConfiguration } from "@/api/configuration";
import { readContent } from "@/api/contents/items";
import { useAccountStore } from "@/stores/account";
import { useToastStore } from "@/stores/toast";

const account = useAccountStore();
const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const toasts = useToastStore();
const { t } = useI18n();

const configuration = ref<Configuration>();
const content = ref<Content>();
const language = ref<Language>();

const breadcrumb = computed<Breadcrumb[]>(() => [{ route: { name: "ContentList" }, text: t("contents.item.title") }]);
const isTypeInvariant = computed<boolean>(() => content.value?.contentType.isInvariant ?? false);
const languages = computed<string[]>(() => content.value?.locales.map((locale) => locale.language?.id ?? "") ?? []);
const title = computed<string>(() => (content.value ? (content.value.invariant.displayName ?? content.value.invariant.uniqueName) : ""));

function addLocale(): void {
  if (content.value && language.value) {
    const actor: Actor = {
      type: account.currentUser ? "User" : "System",
      id: account.currentUser?.id ?? "00000000-0000-0000-0000-000000000000",
      isDeleted: false,
      displayName: account.currentUser?.displayName ?? "System",
      emailAddress: account.currentUser?.emailAddress,
      pictureUrl: account.currentUser?.pictureUrl,
    };
    const now: string = new Date().toISOString();
    content.value.locales.push({
      language: language.value,
      uniqueName: "",
      fieldValues: [],
      createdBy: actor,
      createdOn: now,
      updatedBy: actor,
      updatedOn: now,
      isPublished: false,
    });
    language.value = undefined;
  }
}

function onDeleted(saved: Content, language?: Language): void {
  if (language) {
    if (content.value) {
      content.value.version = saved.version;
      content.value.updatedBy = saved.updatedBy;
      content.value.updatedOn = saved.updatedOn;
      const index: number = content.value.locales.findIndex((locale) => locale.language?.id === language.id);
      if (index >= 0) {
        content.value.locales.splice(index, 1);
      }
    }
    toasts.success("contents.item.deleted.locale");
  } else {
    toasts.success("contents.item.deleted.item");
    router.push({ name: "ContentList" });
  }
}

function onPublished(value: Content, language?: Language): void {
  if (language) {
    // TODO(fpion): implement
  } else {
    // TODO(fpion): implement
  }
  console.log(value); // TODO(fpion): implement
  console.log(language); // TODO(fpion): implement
  toasts.success("contents.item.published.success");
}
function onUnpublished(value: Content, language?: Language): void {
  if (language) {
    // TODO(fpion): implement
  } else {
    // TODO(fpion): implement
  }
  console.log(value); // TODO(fpion): implement
  console.log(language); // TODO(fpion): implement
  toasts.success("contents.item.unpublished.success");
}

function onSaved(saved: Content, language?: Language | null): void {
  if (content.value) {
    content.value.version = saved.version;
    content.value.updatedBy = saved.updatedBy;
    content.value.updatedOn = saved.updatedOn;
    if (language) {
      const locale: ContentLocale | undefined = saved.locales.find((locale) => locale.language?.id === language.id);
      const index: number = content.value.locales.findIndex((locale) => locale.language?.id === language.id);
      if (locale && index >= 0) {
        content.value.locales.splice(index, 1, locale);
      }
    } else {
      content.value.invariant = { ...saved.invariant };
    }
  }
  toasts.success("contents.item.updated");
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
        <PublishButton all class="mx-1" :content="content" @error="handleError" @published="onPublished" />
        <UnpublishButton all class="ms-1" :content="content" @error="handleError" @unpublished="onUnpublished" />
      </div>
      <div class="row">
        <ContentTypeInput class="col" :content-type="content.contentType" />
        <LanguageSelect v-if="!isTypeInvariant" class="col" :exclude="languages" :model-value="language?.id" @selected="language = $event">
          <template #append>
            <TarButton :disabled="!language" icon="fas fa-plus" :text="t('actions.add')" variant="success" @click="addLocale" />
          </template>
        </LanguageSelect>
      </div>
      <ContentLocaleEdit
        v-if="isTypeInvariant"
        :configuration="configuration"
        :content-type="content.contentType"
        :content="content"
        :locale="content.invariant"
        @error="handleError"
        @saved="onSaved"
      />
      <TarTabs v-else>
        <TarTab active id="invariant" :title="t('contents.item.invariant')">
          <ContentLocaleEdit
            :configuration="configuration"
            :content-type="content.contentType"
            :content="content"
            :locale="content.invariant"
            @error="handleError"
            @published="onPublished"
            @saved="onSaved"
            @unpublished="onUnpublished"
          />
        </TarTab>
        <TarTab v-for="locale in content.locales" :id="locale.language?.id" :key="locale.language?.id" :title="locale.language?.locale.displayName">
          <ContentLocaleEdit
            :configuration="configuration"
            :content-type="content.contentType"
            :content="content"
            :locale="locale"
            @deleted="onDeleted($event, locale.language ?? undefined)"
            @error="handleError"
            @published="onPublished($event, locale.language ?? undefined)"
            @saved="onSaved($event, locale.language)"
            @unpublished="onUnpublished($event, locale.language ?? undefined)"
          />
        </TarTab>
      </TarTabs>
    </template>
  </main>
</template>
