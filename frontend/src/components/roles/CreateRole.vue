<script setup lang="ts">
import { TarButton, TarModal } from "logitar-vue3-ui";
import { onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";

import UniqueNameAlreadyUsed from "@/components/shared/UniqueNameAlreadyUsed.vue";
import UniqueNameInput from "@/components/shared/UniqueNameInput.vue";
import type { Configuration } from "@/types/configuration";
import type { CreateOrReplaceRolePayload, Role } from "@/types/roles";
import { ErrorCodes, StatusCodes } from "@/types/api";
import { createRole } from "@/api/roles";
import { isError } from "@/helpers/error";
import { readConfiguration } from "@/api/configuration";

const { t } = useI18n();

const configuration = ref<Configuration>(); // TODO(fpion): get unique name settings from realm (if realm)
const isLoading = ref<boolean>(false);
const modalRef = ref<InstanceType<typeof TarModal> | null>(null);
const uniqueName = ref<string>("");
const uniqueNameAlreadyUsed = ref<boolean>(false);

function hide(): void {
  modalRef.value?.hide();
}

function reset(): void {
  uniqueNameAlreadyUsed.value = false;
  uniqueName.value = "";
}

const emit = defineEmits<{
  (e: "created", value: Role): void;
  (e: "error", value: unknown): void;
}>();

function onCancel(): void {
  reset();
  hide();
}

async function submit(): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    uniqueNameAlreadyUsed.value = false;
    try {
      const payload: CreateOrReplaceRolePayload = {
        uniqueName: uniqueName.value,
        customAttributes: [],
      };
      const role: Role = await createRole(payload);
      emit("created", role);
      reset();
      hide();
    } catch (e: unknown) {
      if (isError(e, StatusCodes.Conflict, ErrorCodes.UniqueNameAlreadyUsed)) {
        uniqueNameAlreadyUsed.value = true;
      } else {
        emit("error", e);
      }
    } finally {
      isLoading.value = false;
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
    <TarButton icon="fas fa-plus" :text="t('actions.create')" variant="success" data-bs-toggle="modal" data-bs-target="#create-role" />
    <TarModal :close="t('actions.close')" id="create-role" ref="modalRef" :title="t('roles.create')">
      <UniqueNameAlreadyUsed v-model="uniqueNameAlreadyUsed" />
      <form @submit.prevent="submit">
        <UniqueNameInput :settings="configuration?.uniqueNameSettings" v-model="uniqueName" />
      </form>
      <template #footer>
        <TarButton icon="fas fa-ban" :text="t('actions.cancel')" variant="secondary" @click="onCancel" />
        <TarButton
          :disabled="!configuration || isLoading"
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
