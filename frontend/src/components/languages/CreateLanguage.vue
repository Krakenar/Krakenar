<script setup lang="ts">
import { TarButton, TarModal } from "logitar-vue3-ui";
import { ref } from "vue";
import { useI18n } from "vue-i18n";

import LocaleAlreadyUsed from "./LocaleAlreadyUsed.vue";
import LocaleSelect from "@/components/shared/LocaleSelect.vue";
import type { CreateOrReplaceLanguagePayload, Language } from "@/types/languages";
import { ErrorCodes, StatusCodes } from "@/types/api";
import { createLanguage } from "@/api/languages";
import { isError } from "@/helpers/error";
import { useForm } from "@/forms";

const { t } = useI18n();

const locale = ref<string>("");
const localeAlreadyUsed = ref<boolean>(false);
const modalRef = ref<InstanceType<typeof TarModal> | null>(null);

function hide(): void {
  modalRef.value?.hide();
}

const emit = defineEmits<{
  (e: "created", value: Language): void;
  (e: "error", value: unknown): void;
}>();

function onCancel(): void {
  onReset();
  hide();
}
function onReset(): void {
  localeAlreadyUsed.value = false;
  reset();
}

const { hasChanges, isSubmitting, handleSubmit, reset } = useForm();
async function submit(): Promise<void> {
  localeAlreadyUsed.value = false;
  try {
    const payload: CreateOrReplaceLanguagePayload = {
      locale: locale.value,
    };
    const language: Language = await createLanguage(payload);
    emit("created", language);
    onReset();
    hide();
  } catch (e: unknown) {
    if (isError(e, StatusCodes.Conflict, ErrorCodes.LocaleAlreadyUsed)) {
      localeAlreadyUsed.value = true;
    } else {
      emit("error", e);
    }
  }
}
</script>

<template>
  <span>
    <TarButton icon="fas fa-plus" :text="t('actions.create')" variant="success" data-bs-toggle="modal" data-bs-target="#create-language" />
    <TarModal :close="t('actions.close')" id="create-language" ref="modalRef" :title="t('languages.create')">
      <LocaleAlreadyUsed v-model="localeAlreadyUsed" />
      <form @submit.prevent="handleSubmit(submit)">
        <LocaleSelect required v-model="locale" />
      </form>
      <template #footer>
        <TarButton icon="fas fa-ban" :text="t('actions.cancel')" variant="secondary" @click="onCancel" />
        <TarButton
          :disabled="isSubmitting || !hasChanges"
          icon="fas fa-plus"
          :loading="isSubmitting"
          :status="t('loading')"
          :text="t('actions.create')"
          type="submit"
          variant="success"
          @click="handleSubmit(submit)"
        />
      </template>
    </TarModal>
  </span>
</template>
