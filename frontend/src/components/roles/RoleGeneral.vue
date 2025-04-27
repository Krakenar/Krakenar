<script setup lang="ts">
import { TarButton } from "logitar-vue3-ui";
import { ref, watch } from "vue";
import { useI18n } from "vue-i18n";

import DescriptionTextarea from "@/components/shared/DescriptionTextarea.vue";
import DisplayNameInput from "@/components/shared/DisplayNameInput.vue";
import UniqueNameAlreadyUsed from "@/components/shared/UniqueNameAlreadyUsed.vue";
import UniqueNameInput from "@/components/shared/UniqueNameInput.vue";
import type { Role, UpdateRolePayload } from "@/types/roles";
import type { UniqueNameSettings } from "@/types/settings";
import { ErrorCodes, StatusCodes } from "@/types/api";
import { isError } from "@/helpers/error";
import { updateRole } from "@/api/roles";

const { t } = useI18n();

const props = defineProps<{
  role: Role;
}>();

const description = ref<string>("");
const displayName = ref<string>("");
const isLoading = ref<boolean>(false);
const uniqueName = ref<string>("");
const uniqueNameAlreadyUsed = ref<boolean>(false);
const uniqueNameSettings = ref<UniqueNameSettings>({}); // TODO(fpion): get from realm or configuration

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "updated", value: Role): void;
}>();

async function submit(): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    uniqueNameAlreadyUsed.value = false;
    try {
      const payload: UpdateRolePayload = {
        uniqueName: props.role.uniqueName !== uniqueName.value ? uniqueName.value : undefined,
        displayName: (props.role.displayName ?? "") !== displayName.value ? { value: displayName.value } : undefined,
        description: (props.role.description ?? "") !== description.value ? { value: description.value } : undefined,
        customAttributes: [],
      };
      const role: Role = await updateRole(props.role.id, payload);
      emit("updated", role);
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

watch(
  () => props.role,
  (role) => {
    uniqueName.value = role.uniqueName;
    displayName.value = role.displayName ?? "";
    description.value = role.description ?? "";
  },
  { deep: true, immediate: true },
);
</script>

<template>
  <div>
    <form @submit.prevent="submit">
      <UniqueNameAlreadyUsed v-model="uniqueNameAlreadyUsed" />
      <div class="row">
        <UniqueNameInput class="col" :settings="uniqueNameSettings" v-model="uniqueName" />
        <DisplayNameInput class="col" v-model="displayName" />
      </div>
      <DescriptionTextarea v-model="description" />
      <div class="mb-3">
        <TarButton :disabled="isLoading" icon="fas fa-save" :loading="isLoading" :status="t('loading')" :text="t('actions.save')" type="submit" />
      </div>
    </form>
  </div>
</template>
