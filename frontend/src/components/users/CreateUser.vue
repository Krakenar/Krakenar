<script setup lang="ts">
import { TarButton, TarModal } from "logitar-vue3-ui";
import { computed, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";

import UniqueNameAlreadyUsed from "@/components/shared/UniqueNameAlreadyUsed.vue";
import UniqueNameInput from "@/components/shared/UniqueNameInput.vue";
import type { Configuration } from "@/types/configuration";
import type { CreateOrReplaceUserPayload, User } from "@/types/users";
import type { UniqueNameSettings } from "@/types/settings";
import { ErrorCodes, StatusCodes } from "@/types/api";
import { createUser } from "@/api/users";
import { isError } from "@/helpers/error";
import { readConfiguration } from "@/api/configuration";
import { useForm } from "@/forms";
import { useRealmStore } from "@/stores/realm";

const realm = useRealmStore();
const { t } = useI18n();

const configuration = ref<Configuration>();
const modalRef = ref<InstanceType<typeof TarModal> | null>(null);
const uniqueName = ref<string>("");
const uniqueNameAlreadyUsed = ref<boolean>(false);

const uniqueNameSettings = computed<UniqueNameSettings | undefined>(() => realm.currentRealm?.uniqueNameSettings ?? configuration.value?.uniqueNameSettings);

function hide(): void {
  modalRef.value?.hide();
}

const emit = defineEmits<{
  (e: "created", value: User): void;
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
    const payload: CreateOrReplaceUserPayload = {
      uniqueName: uniqueName.value,
      customAttributes: [],
      roles: [],
    };
    const user: User = await createUser(payload);
    emit("created", user);
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
    <TarButton icon="fas fa-plus" :text="t('actions.create')" variant="success" data-bs-toggle="modal" data-bs-target="#create-user" />
    <TarModal :close="t('actions.close')" id="create-user" ref="modalRef" size="large" :title="t('users.create')">
      <UniqueNameAlreadyUsed v-model="uniqueNameAlreadyUsed" />
      <form @submit.prevent="handleSubmit(submit)">
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
