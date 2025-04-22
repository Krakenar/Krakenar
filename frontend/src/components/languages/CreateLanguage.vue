<script setup lang="ts">
import { TarButton, TarModal } from "logitar-vue3-ui";
import { ref } from "vue";
import { useI18n } from "vue-i18n";

import type { CreateOrReplaceLanguagePayload, Language } from "@/types/languages";
import { ErrorCodes, StatusCodes } from "@/types/api";
import { createLanguage } from "@/api/languages";
import { isError } from "@/helpers/error";

const { t } = useI18n();

const isLoading = ref<boolean>(false);
const locale = ref<string>("");
const localeAlreadyUsed = ref<boolean>(false);
const modalRef = ref<InstanceType<typeof TarModal> | null>(null);

function hide(): void {
  modalRef.value?.hide();
}

function reset(): void {
  localeAlreadyUsed.value = false;
  locale.value = "";
}

const emit = defineEmits<{
  (e: "created", value: Language): void;
  (e: "error", value: unknown): void;
}>();

function onCancel(): void {
  reset();
  hide();
}

async function submit(): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    localeAlreadyUsed.value = false;
    try {
      const payload: CreateOrReplaceLanguagePayload = {
        locale: locale.value,
      };
      const language: Language = await createLanguage(payload);
      emit("created", language);
      reset();
      hide();
    } catch (e: unknown) {
      if (isError(e, StatusCodes.Conflict, ErrorCodes.LocaleAlreadyUsed)) {
        localeAlreadyUsed.value = true;
      } else {
        emit("error", e);
      }
    } finally {
      isLoading.value = false;
    }
  }
}
</script>

<template>
  <span>
    <TarButton icon="fas fa-plus" :text="t('actions.create')" variant="success" data-bs-toggle="modal" data-bs-target="#create-language" />
    <TarModal :close="t('actions.close')" id="create-language" ref="modalRef" :title="t('languages.create')">
      <!-- TODO(fpion): <LocaleAlreadyUsed v-model="localeAlreadyUsed" />-->
      <form @submit.prevent="submit">
        <!-- TODO(fpion): LocaleSelect -->
      </form>
      <template #footer>
        <TarButton icon="fas fa-ban" :text="t('actions.cancel')" variant="secondary" @click="onCancel" />
        <TarButton
          :disabled="isLoading"
          icon="fas fa-plus"
          :loading="isLoading"
          :status="t('loading')"
          :text="t('actions.create')"
          type="submit"
          variant="success"
          @click="submit"
        />
      </template>
    </TarModal>
  </span>
</template>
