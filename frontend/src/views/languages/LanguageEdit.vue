<script setup lang="ts">
import { TarButton } from "logitar-vue3-ui";
import { inject, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";
import { useRoute } from "vue-router";

import DefaultButton from "@/components/languages/DefaultButton.vue";
import LocaleAlreadyUsed from "@/components/languages/LocaleAlreadyUsed.vue";
import LocaleSelect from "@/components/shared/LocaleSelect.vue";
import StatusDetail from "@/components/shared/StatusDetail.vue";
import type { Language } from "@/types/languages";
import type { UpdateLanguagePayload } from "@/types/languages";
import { ErrorCodes, StatusCodes } from "@/types/api";
import { formatLocale } from "@/helpers/format";
import { handleErrorKey } from "@/inject/App";
import { isError } from "@/helpers/error";
import { readLanguage } from "@/api/languages";
import { updateLanguage } from "@/api/languages";
import { useToastStore } from "@/stores/toast";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const toasts = useToastStore();
const { t } = useI18n();

const isLoading = ref<boolean>(false);
const language = ref<Language>();
const locale = ref<string>("");
const localeAlreadyUsed = ref<boolean>(false);

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

async function submit(): Promise<void> {
  if (!isLoading.value && language.value) {
    isLoading.value = true;
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
    } finally {
      isLoading.value = false;
    }
  }
}

onMounted(async () => {
  try {
    const id = route.params.id as string;
    const language: Language = await readLanguage(id);
    setModel(language);
  } catch (e: unknown) {
    handleError(e);
  }
});
</script>

<template>
  <main class="container">
    <template v-if="language">
      <h1>{{ formatLocale(language.locale) }}</h1>
      <StatusDetail :aggregate="language" />
      <div class="mb-3">
        <DefaultButton :language="language" @error="handleError" @saved="onSetDefault" />
      </div>
      <LocaleAlreadyUsed v-model="localeAlreadyUsed" />
      <form @submit.prevent="submit">
        <LocaleSelect required v-model="locale" />
        <div class="mb-3">
          <TarButton :disabled="isLoading" icon="fas fa-save" :loading="isLoading" :status="t('loading')" :text="t('actions.save')" type="submit" />
        </div>
      </form>
    </template>
  </main>
</template>
