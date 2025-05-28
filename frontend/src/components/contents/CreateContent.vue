<script setup lang="ts">
import { TarButton, TarModal } from "logitar-vue3-ui";
import { computed, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";

import ContentTypeFormSelect from "./ContentTypeFormSelect.vue";
import LanguageFormSelect from "@/components/languages/LanguageFormSelect.vue";
import UniqueNameAlreadyUsed from "@/components/shared/UniqueNameAlreadyUsed.vue";
import UniqueNameInput from "@/components/shared/UniqueNameInput.vue";
import type { Configuration } from "@/types/configuration";
import type { Content, ContentType, CreateContentPayload } from "@/types/contents";
import type { Language } from "@/types/languages";
import { ErrorCodes, StatusCodes } from "@/types/api";
import { createContent } from "@/api/contents/items";
import { isError } from "@/helpers/error";
import { readConfiguration } from "@/api/configuration";
import { useForm } from "@/forms";
import { useRealmStore } from "@/stores/realm";
import type { UniqueNameSettings } from "@/types/settings";

const realm = useRealmStore();
const { t } = useI18n();

const configuration = ref<Configuration>();
const contentType = ref<ContentType>();
const language = ref<Language>();
const modalRef = ref<InstanceType<typeof TarModal> | null>(null);
const uniqueName = ref<string>("");
const uniqueNameAlreadyUsed = ref<boolean>(false);

const isInvariant = computed<boolean>(() => contentType.value?.isInvariant ?? false);
const uniqueNameSettings = computed<UniqueNameSettings | undefined>(() => realm.currentRealm?.uniqueNameSettings ?? configuration.value?.uniqueNameSettings);

function hide(): void {
  modalRef.value?.hide();
}

const emit = defineEmits<{
  (e: "created", value: Content): void;
  (e: "error", value: unknown): void;
}>();

function onCancel(): void {
  onReset();
  hide();
}
function onReset(): void {
  uniqueNameAlreadyUsed.value = false;
  reset();
}

function onContentTypeSelected(selected: ContentType | undefined) {
  contentType.value = selected;
  if (!isInvariant.value) {
    language.value = undefined;
  }
}

const { hasChanges, isSubmitting, handleSubmit, reset } = useForm();
async function submit(): Promise<void> {
  if (contentType.value) {
    uniqueNameAlreadyUsed.value = false;
    try {
      const payload: CreateContentPayload = {
        contentType: contentType.value.id,
        language: language.value?.id,
        uniqueName: uniqueName.value,
        fieldValues: [],
      };
      const content: Content = await createContent(payload);
      emit("created", content);
      onReset();
      hide();
    } catch (e: unknown) {
      if (isError(e, StatusCodes.Conflict, ErrorCodes.ContentUniqueNameAlreadyUsed)) {
        uniqueNameAlreadyUsed.value = true;
      } else {
        emit("error", e);
      }
    }
  }
}

onMounted(async () => {
  try {
    if (!realm.currentRealm) {
      configuration.value = await readConfiguration();
    }
  } catch (e: unknown) {
    emit("error", e);
  }
});
</script>

<template>
  <span>
    <TarButton icon="fas fa-plus" :text="t('actions.create')" variant="success" data-bs-toggle="modal" data-bs-target="#create-content" />
    <TarModal :close="t('actions.close')" id="create-content" ref="modalRef" :title="t('contents.item.create')">
      <UniqueNameAlreadyUsed v-model="uniqueNameAlreadyUsed" />
      <form @submit.prevent="handleSubmit(submit)">
        <ContentTypeFormSelect :model-value="contentType?.id" required @selected="onContentTypeSelected" />
        <LanguageFormSelect :disabled="isInvariant" :model-value="language?.id" :required="!isInvariant" @selected="language = $event" />
        <UniqueNameInput required :settings="uniqueNameSettings" v-model="uniqueName" />
      </form>
      <template #footer>
        <TarButton icon="fas fa-ban" :text="t('actions.cancel')" variant="secondary" @click="onCancel" />
        <TarButton
          :disabled="!uniqueNameSettings || isSubmitting || !hasChanges"
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
