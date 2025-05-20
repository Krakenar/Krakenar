<script setup lang="ts">
import { TarButton } from "logitar-vue3-ui";
import { computed, inject, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import AppBreadcrumb from "@/components/shared/AppBreadcrumb.vue";
import DeleteDictionary from "@/components/dictionaries/DeleteDictionary.vue";
import DictionaryEntryList from "@/components/dictionaries/DictionaryEntryList.vue";
import LanguageAlreadyUsed from "@/components/languages/LanguageAlreadyUsed.vue";
import LanguageSelect from "@/components/languages/LanguageSelect.vue";
import StatusDetail from "@/components/shared/StatusDetail.vue";
import type { Breadcrumb } from "@/types/breadcrumb";
import type { CreateOrReplaceDictionaryPayload, Dictionary, DictionaryEntry } from "@/types/dictionaries";
import type { Language } from "@/types/languages";
import { ErrorCodes, StatusCodes, type ApiFailure } from "@/types/api";
import { formatLocale } from "@/helpers/format";
import { handleErrorKey } from "@/inject/App";
import { isError } from "@/helpers/error";
import { readDictionary } from "@/api/dictionaries";
import { replaceDictionary } from "@/api/dictionaries";
import { useForm } from "@/forms";
import { useToastStore } from "@/stores/toast";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const toasts = useToastStore();
const { t } = useI18n();

const dictionary = ref<Dictionary>();
const entries = ref<DictionaryEntry[]>([]);
const language = ref<Language>();
const languageAlreadyUsed = ref<boolean>();

const breadcrumb = computed<Breadcrumb[]>(() => [{ route: { name: "DictionaryList" }, text: t("dictionaries.title") }]);
const hasChanges = computed<boolean>(() => hasFormChanges.value || entries.value.some((entry) => entry.isRemoved));
const title = computed<string>(() => (dictionary.value ? formatLocale(dictionary.value.language.locale) : ""));

function onDeleted(): void {
  toasts.success("dictionaries.deleted");
  router.push({ name: "DictionaryList" });
}

function setModel(model: Dictionary): void {
  dictionary.value = model;
  language.value = model.language;
  entries.value = model.entries;
}

const { hasChanges: hasFormChanges, isSubmitting, handleSubmit } = useForm();
async function submit(): Promise<void> {
  if (dictionary.value && language.value) {
    languageAlreadyUsed.value = false;
    try {
      const payload: CreateOrReplaceDictionaryPayload = {
        language: language.value.id,
        entries: entries.value.filter((entry) => !entry.isRemoved),
      };
      const updated: Dictionary = await replaceDictionary(dictionary.value.id, payload, dictionary.value.version);
      setModel(updated);
      toasts.success("dictionaries.updated");
    } catch (e: unknown) {
      if (isError(e, StatusCodes.Conflict, ErrorCodes.LanguageAlreadyUsed)) {
        languageAlreadyUsed.value = true;
      } else {
        handleError(e);
      }
    }
  }
}

onMounted(async () => {
  try {
    const id = route.params.id as string;
    const dictionary: Dictionary = await readDictionary(id);
    setModel(dictionary);
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
    <template v-if="dictionary">
      <h1>{{ title }}</h1>
      <AppBreadcrumb :current="title" :parent="breadcrumb" />
      <StatusDetail :aggregate="dictionary" />
      <div class="mb-3">
        <DeleteDictionary :dictionary="dictionary" @deleted="onDeleted" @error="handleError" />
      </div>
      <form @submit.prevent="handleSubmit(submit)">
        <LanguageAlreadyUsed v-model="languageAlreadyUsed" />
        <LanguageSelect :model-value="language?.id" required @selected="language = $event" />
        <DictionaryEntryList v-model="entries" />
        <div class="mb-3">
          <TarButton
            :disabled="isSubmitting || !hasChanges"
            icon="fas fa-save"
            :loading="isSubmitting"
            :status="t('loading')"
            :text="t('actions.save')"
            type="submit"
          />
        </div>
      </form>
    </template>
  </main>
</template>
