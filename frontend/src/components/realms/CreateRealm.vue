<script setup lang="ts">
import { TarButton, TarModal } from "logitar-vue3-ui";
import { ref } from "vue";
import { useI18n } from "vue-i18n";

import UniqueSlugAlreadyUsed from "./UniqueSlugAlreadyUsed.vue";
import UniqueSlugInput from "./UniqueSlugInput.vue";
import type { CreateOrReplaceRealmPayload, Realm } from "@/types/realms";
import { ErrorCodes, StatusCodes } from "@/types/api";
import { createRealm } from "@/api/realms";
import { isError } from "@/helpers/error";
import { useForm } from "@/forms";

const { t } = useI18n();

const modalRef = ref<InstanceType<typeof TarModal> | null>(null);
const uniqueSlug = ref<string>("");
const uniqueSlugAlreadyUsed = ref<boolean>(false);

function hide(): void {
  modalRef.value?.hide();
}

const emit = defineEmits<{
  (e: "created", value: Realm): void;
  (e: "error", value: unknown): void;
}>();

function onCancel(): void {
  onReset();
  hide();
}
function onReset(): void {
  uniqueSlugAlreadyUsed.value = false;
  reset();
}

const { hasChanges, isSubmitting, handleSubmit, reset } = useForm();
async function submit(): Promise<void> {
  uniqueSlugAlreadyUsed.value = false;
  try {
    const payload: CreateOrReplaceRealmPayload = {
      uniqueSlug: uniqueSlug.value,
      uniqueNameSettings: {
        allowedCharacters: "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+",
      },
      passwordSettings: {
        requiredLength: 8,
        requiredUniqueChars: 8,
        requireNonAlphanumeric: true,
        requireLowercase: true,
        requireUppercase: true,
        requireDigit: true,
        hashingStrategy: "PBKDF2",
      },
      requireUniqueEmail: true,
      requireConfirmedAccount: true,
      customAttributes: [],
    };
    const realm: Realm = await createRealm(payload);
    emit("created", realm);
    onReset();
    hide();
  } catch (e: unknown) {
    if (isError(e, StatusCodes.Conflict, ErrorCodes.UniqueSlugAlreadyUsed)) {
      uniqueSlugAlreadyUsed.value = true;
    } else {
      emit("error", e);
    }
  }
}
</script>

<template>
  <span>
    <TarButton icon="fas fa-plus" :text="t('actions.create')" variant="success" data-bs-toggle="modal" data-bs-target="#create-realm" />
    <TarModal :close="t('actions.close')" id="create-realm" ref="modalRef" :title="t('realms.create')">
      <UniqueSlugAlreadyUsed v-model="uniqueSlugAlreadyUsed" />
      <form @submit.prevent="handleSubmit(submit)">
        <UniqueSlugInput required v-model="uniqueSlug" />
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
