<script setup lang="ts">
import { TarButton, TarModal } from "logitar-vue3-ui";
import { ref } from "vue";
import { useI18n } from "vue-i18n";

import LanguageAlreadyUsed from "@/components/languages/LanguageAlreadyUsed.vue";
import LanguageSelect from "@/components/languages/LanguageSelect.vue";
import type { CreateOrReplaceDictionaryPayload, Dictionary } from "@/types/dictionaries";
import type { Language } from "@/types/languages";
import { ErrorCodes, StatusCodes } from "@/types/api";
import { createDictionary } from "@/api/dictionaries";
import { isError } from "@/helpers/error";
import { useForm } from "@/forms";

const { t } = useI18n();

const language = ref<Language>();
const languageAlreadyUsed = ref<boolean>(false);
const modalRef = ref<InstanceType<typeof TarModal> | null>(null);

function hide(): void {
  modalRef.value?.hide();
}

const emit = defineEmits<{
  (e: "created", value: Dictionary): void;
  (e: "error", value: unknown): void;
}>();

function onCancel(): void {
  onReset();
  hide();
}
function onReset(): void {
  languageAlreadyUsed.value = false;
  reset();
}

const { hasChanges, isSubmitting, handleSubmit, reset } = useForm();
async function submit(): Promise<void> {
  if (language.value) {
    languageAlreadyUsed.value = false;
    try {
      const payload: CreateOrReplaceDictionaryPayload = {
        language: language.value.id,
        entries: [],
      };
      const dictionary: Dictionary = await createDictionary(payload);
      emit("created", dictionary);
      onReset();
      hide();
    } catch (e: unknown) {
      if (isError(e, StatusCodes.Conflict, ErrorCodes.LanguageAlreadyUsed)) {
        languageAlreadyUsed.value = true;
      } else {
        emit("error", e);
      }
    }
  }
}
</script>

<template>
  <span>
    <TarButton icon="fas fa-plus" :text="t('actions.create')" variant="success" data-bs-toggle="modal" data-bs-target="#create-dictionary" />
    <TarModal :close="t('actions.close')" id="create-dictionary" ref="modalRef" :title="t('dictionaries.create')">
      <LanguageAlreadyUsed v-model="languageAlreadyUsed" />
      <form @submit.prevent="handleSubmit(submit)">
        <LanguageSelect :model-value="language?.id" required @selected="language = $event" />
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
