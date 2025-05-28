<script setup lang="ts">
import { TarButton, TarModal } from "logitar-vue3-ui";
import { computed, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";

import ContentTypeFormSelect from "@/components/contents/ContentTypeFormSelect.vue";
import DataTypeFormSelect from "./DataTypeFormSelect.vue";
import UniqueNameAlreadyUsed from "@/components/shared/UniqueNameAlreadyUsed.vue";
import UniqueNameInput from "@/components/shared/UniqueNameInput.vue";
import type { Configuration } from "@/types/configuration";
import type { ContentType } from "@/types/contents";
import type { FieldType, CreateOrReplaceFieldTypePayload, DataType } from "@/types/fields";
import type { UniqueNameSettings } from "@/types/settings";
import { ErrorCodes, StatusCodes } from "@/types/api";
import { createFieldType } from "@/api/fields/types";
import { isError } from "@/helpers/error";
import { readConfiguration } from "@/api/configuration";
import { useForm } from "@/forms";
import { useRealmStore } from "@/stores/realm";

const realm = useRealmStore();
const { t } = useI18n();

const configuration = ref<Configuration>();
const contentType = ref<ContentType>();
const dataType = ref<string>("");
const modalRef = ref<InstanceType<typeof TarModal> | null>(null);
const uniqueName = ref<string>("");
const uniqueNameAlreadyUsed = ref<boolean>(false);

const uniqueNameSettings = computed<UniqueNameSettings | undefined>(() => realm.currentRealm?.uniqueNameSettings ?? configuration.value?.uniqueNameSettings);

function hide(): void {
  modalRef.value?.hide();
}

const emit = defineEmits<{
  (e: "created", value: FieldType): void;
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

const { hasChanges, isSubmitting, handleSubmit, reset } = useForm();
async function submit(): Promise<void> {
  uniqueNameAlreadyUsed.value = false;
  try {
    const payload: CreateOrReplaceFieldTypePayload = {
      uniqueName: uniqueName.value,
    };
    switch (dataType.value as DataType) {
      case "Boolean":
        payload.boolean = {};
        break;
      case "DateTime":
        payload.dateTime = {};
        break;
      case "Number":
        payload.number = {};
        break;
      case "RelatedContent":
        if (contentType.value) {
          payload.relatedContent = { contentTypeId: contentType.value.id, isMultiple: false };
        }
        break;
      case "RichText":
        payload.richText = { contentType: "text/plain" };
        break;
      case "Select":
        payload.select = { options: [], isMultiple: false };
        break;
      case "String":
        payload.string = {};
        break;
      case "Tags":
        payload.tags = {};
        break;
      default:
        throw new Error(`The data type ${dataType.value} is not supported.`);
    }
    const fieldType: FieldType = await createFieldType(payload);
    emit("created", fieldType);
    onReset();
    hide();
  } catch (e: unknown) {
    if (isError(e, StatusCodes.Conflict, ErrorCodes.UniqueNameAlreadyUsed)) {
      uniqueNameAlreadyUsed.value = true;
    } else {
      emit("error", e);
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
    <TarButton icon="fas fa-plus" :text="t('actions.create')" variant="success" data-bs-toggle="modal" data-bs-target="#create-field-type" />
    <TarModal :close="t('actions.close')" id="create-field-type" size="large" ref="modalRef" :title="t('fields.type.create')">
      <UniqueNameAlreadyUsed v-model="uniqueNameAlreadyUsed" />
      <form @submit.prevent="handleSubmit(submit)">
        <UniqueNameInput required :settings="uniqueNameSettings" v-model="uniqueName" />
        <DataTypeFormSelect required v-model="dataType" />
        <ContentTypeFormSelect v-if="dataType === 'RelatedContent'" :model-value="contentType?.id" required @selected="contentType = $event" />
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
