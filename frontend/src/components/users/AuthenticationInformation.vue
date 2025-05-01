<script setup lang="ts">
import { TarButton } from "logitar-vue3-ui";
import { computed, ref, watch } from "vue";
import { useI18n } from "vue-i18n";

import UniqueNameAlreadyUsed from "@/components/shared/UniqueNameAlreadyUsed.vue";
import UsernameInput from "./UsernameInput.vue";
import type { Configuration } from "@/types/configuration";
import type { UniqueNameSettings } from "@/types/settings";
import type { UpdateUserPayload, User } from "@/types/users";
import { ErrorCodes, StatusCodes } from "@/types/api";
import { isError } from "@/helpers/error";
import { updateUser } from "@/api/users";
import { useForm } from "@/forms";

const { t } = useI18n();

const props = defineProps<{
  configuration?: Configuration;
  user: User;
}>();

const uniqueName = ref<string>("");
const uniqueNameAlreadyUsed = ref<boolean>(false);

const uniqueNameSettings = computed<UniqueNameSettings | undefined>(() => props.user.realm?.uniqueNameSettings ?? props.configuration?.uniqueNameSettings);

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "updated", value: User): void;
}>();

const { hasChanges, isSubmitting, handleSubmit } = useForm();
async function submit(): Promise<void> {
  uniqueNameAlreadyUsed.value = false;
  try {
    const payload: UpdateUserPayload = {
      uniqueName: props.user.uniqueName !== uniqueName.value ? uniqueName.value : undefined,
      customAttributes: [],
      roles: [],
    };
    const user: User = await updateUser(props.user.id, payload);
    emit("updated", user);
  } catch (e: unknown) {
    if (isError(e, StatusCodes.Conflict, ErrorCodes.UniqueNameAlreadyUsed)) {
      uniqueNameAlreadyUsed.value = true;
    } else {
      emit("error", e);
    }
  }
}

watch(
  () => props.user,
  (user) => {
    uniqueName.value = user.uniqueName;
  },
  { deep: true, immediate: true },
);
</script>

<template>
  <form @submit.prevent="handleSubmit(submit)">
    <UniqueNameAlreadyUsed v-model="uniqueNameAlreadyUsed" />
    <UsernameInput :settings="uniqueNameSettings" v-model="uniqueName" />
    <div class="mb-3">
      <TarButton
        :disabled="isSubmitting || !hasChanges"
        icon="fas fa-save"
        :loading="isSubmitting"
        :status="t('loading')"
        :text="t('actions.save')"
        type="submit"
      />
    </div>
  </form>
</template>
