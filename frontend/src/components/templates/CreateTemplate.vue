<script setup lang="ts">
import { TarButton, TarModal } from "logitar-vue3-ui";
import { onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";

import UniqueNameAlreadyUsed from "@/components/shared/UniqueNameAlreadyUsed.vue";
import UniqueNameInput from "@/components/shared/UniqueNameInput.vue";
import type { Configuration } from "@/types/configuration";
import type { CreateOrReplaceTemplatePayload, Template } from "@/types/templates";
import { ErrorCodes, StatusCodes } from "@/types/api";
import { createTemplate } from "@/api/templates";
import { isError } from "@/helpers/error";
import { readConfiguration } from "@/api/configuration";
import { useForm } from "@/forms";

const { t } = useI18n();

const configuration = ref<Configuration>(); // TODO(fpion): get unique name settings from realm (if realm)
const modalRef = ref<InstanceType<typeof TarModal> | null>(null);
const uniqueName = ref<string>("");
const uniqueNameAlreadyUsed = ref<boolean>(false);

function hide(): void {
  modalRef.value?.hide();
}

const emit = defineEmits<{
  (e: "created", value: Template): void;
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
    const payload: CreateOrReplaceTemplatePayload = {
      uniqueName: uniqueName.value,
      subject: `${uniqueName.value}_Subject`,
      content: {
        type: "text/plain",
        text: "This is your message body.",
      },
    };
    const template: Template = await createTemplate(payload);
    emit("created", template);
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
    configuration.value = await readConfiguration();
  } catch (e: unknown) {
    emit("error", e);
  }
});
</script>

<template>
  <span>
    <TarButton icon="fas fa-plus" :text="t('actions.create')" variant="success" data-bs-toggle="modal" data-bs-target="#create-template" />
    <TarModal :close="t('actions.close')" id="create-template" ref="modalRef" :title="t('templates.create')">
      <UniqueNameAlreadyUsed v-model="uniqueNameAlreadyUsed" />
      <form @submit.prevent="handleSubmit(submit)">
        <UniqueNameInput required :settings="configuration?.uniqueNameSettings" v-model="uniqueName" />
      </form>
      <template #footer>
        <TarButton icon="fas fa-ban" :text="t('actions.cancel')" variant="secondary" @click="onCancel" />
        <TarButton
          :disabled="!configuration || isSubmitting || !hasChanges"
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
