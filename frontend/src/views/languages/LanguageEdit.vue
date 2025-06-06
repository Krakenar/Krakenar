<script setup lang="ts">
import { TarButton } from "logitar-vue3-ui";
import { computed, inject, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import AppBreadcrumb from "@/components/shared/AppBreadcrumb.vue";
import DefaultButton from "@/components/languages/DefaultButton.vue";
import DeleteLanguage from "@/components/languages/DeleteLanguage.vue";
import LocaleAlreadyUsed from "@/components/languages/LocaleAlreadyUsed.vue";
import LocaleSelect from "@/components/shared/LocaleSelect.vue";
import StatusDetail from "@/components/shared/StatusDetail.vue";
import type { Breadcrumb } from "@/types/breadcrumb";
import type { Language } from "@/types/languages";
import type { UpdateLanguagePayload } from "@/types/languages";
import { ErrorCodes, StatusCodes, type ApiFailure } from "@/types/api";
import { formatLocale } from "@/helpers/format";
import { handleErrorKey } from "@/inject/App";
import { isError } from "@/helpers/error";
import { readLanguage } from "@/api/languages";
import { updateLanguage } from "@/api/languages";
import { useForm } from "@/forms";
import { useToastStore } from "@/stores/toast";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const toasts = useToastStore();
const { t } = useI18n();

const language = ref<Language>();
const locale = ref<string>("");
const localeAlreadyUsed = ref<boolean>(false);

const breadcrumb = computed<Breadcrumb[]>(() => [{ route: { name: "LanguageList" }, text: t("languages.title") }]);
const title = computed<string>(() => (language.value ? formatLocale(language.value.locale) : ""));

function onDeleted(): void {
  toasts.success("languages.deleted");
  router.push({ name: "LanguageList" });
}

function setModel(model: Language): void {
  language.value = model;
  locale.value = model.locale.code;
}

function onSetDefault(updated: Language): void {
  if (language.value) {
    language.value.version = updated.version;
    language.value.updatedBy = updated.updatedBy;
    language.value.updatedOn = updated.updatedOn;
    language.value.isDefault = updated.isDefault;
  }
  toasts.success("languages.default.set");
}

const { hasChanges, isSubmitting, handleSubmit } = useForm();
async function submit(): Promise<void> {
  if (language.value) {
    localeAlreadyUsed.value = false;
    try {
      const payload: UpdateLanguagePayload = {
        locale: language.value.locale.code !== locale.value ? locale.value : undefined,
      };
      const updatedLanguage: Language = await updateLanguage(language.value.id, payload);
      setModel(updatedLanguage);
      toasts.success("languages.updated");
    } catch (e: unknown) {
      if (isError(e, StatusCodes.Conflict, ErrorCodes.LocaleAlreadyUsed)) {
        localeAlreadyUsed.value = true;
      } else {
        handleError(e);
      }
    }
  }
}

onMounted(async () => {
  try {
    const id = route.params.id as string;
    const language: Language = await readLanguage(id);
    setModel(language);
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
    <template v-if="language">
      <h1>{{ title }}</h1>
      <AppBreadcrumb :current="title" :parent="breadcrumb" />
      <StatusDetail :aggregate="language" />
      <div class="mb-3">
        <DeleteLanguage class="me-1" :disabled="language.isDefault" :language="language" @deleted="onDeleted" @error="handleError" />
        <DefaultButton class="ms-1" :language="language" @error="handleError" @saved="onSetDefault" />
      </div>
      <LocaleAlreadyUsed v-model="localeAlreadyUsed" />
      <form @submit.prevent="handleSubmit(submit)">
        <LocaleSelect required v-model="locale" />
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
