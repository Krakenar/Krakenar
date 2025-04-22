<script setup lang="ts">
import { TarButton, TarModal } from "logitar-vue3-ui";
import { ref } from "vue";
import { useI18n } from "vue-i18n";

import DisplayNameInput from "@/components/shared/DisplayNameInput.vue";
import UniqueSlugAlreadyUsed from "./UniqueSlugAlreadyUsed.vue";
import UniqueSlugInput from "./UniqueSlugInput.vue";
import type { CreateOrReplaceRealmPayload, Realm } from "@/types/realms";
import { ErrorCodes, StatusCodes } from "@/types/api";
import { createRealm } from "@/api/realms";
import { isError } from "@/helpers/error";

const { t } = useI18n();

const displayName = ref<string>("");
const isLoading = ref<boolean>(false);
const modalRef = ref<InstanceType<typeof TarModal> | null>(null);
const uniqueSlug = ref<string>("");
const uniqueSlugAlreadyUsed = ref<boolean>(false);

function hide(): void {
  modalRef.value?.hide();
}

function reset(): void {
  uniqueSlugAlreadyUsed.value = false;
  displayName.value = "";
  uniqueSlug.value = "";
}

const emit = defineEmits<{
  (e: "created", value: Realm): void;
  (e: "error", value: unknown): void;
}>();

function onCancel(): void {
  reset();
  hide();
}

async function submit(): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    uniqueSlugAlreadyUsed.value = false;
    try {
      const payload: CreateOrReplaceRealmPayload = {
        uniqueSlug: uniqueSlug.value,
        displayName: displayName.value,
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
      reset();
      hide();
    } catch (e: unknown) {
      if (isError(e, StatusCodes.Conflict, ErrorCodes.UniqueSlugAlreadyUsed)) {
        uniqueSlugAlreadyUsed.value = true;
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
    <TarButton icon="fas fa-plus" :text="t('actions.create')" variant="success" data-bs-toggle="modal" data-bs-target="#create-realm" />
    <TarModal :close="t('actions.close')" id="create-realm" ref="modalRef" :title="t('realms.create')">
      <UniqueSlugAlreadyUsed v-model="uniqueSlugAlreadyUsed" />
      <form @submit.prevent="submit">
        <DisplayNameInput v-model="displayName" />
        <UniqueSlugInput :name-value="displayName" required v-model="uniqueSlug" />
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
