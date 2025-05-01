<script setup lang="ts">
import { TarButton } from "logitar-vue3-ui";
import { computed, ref, watch } from "vue";
import { useI18n } from "vue-i18n";

import PasswordInput from "./PasswordInput.vue";
import StatusInfo from "@/components/shared/StatusInfo.vue";
import UniqueNameAlreadyUsed from "@/components/shared/UniqueNameAlreadyUsed.vue";
import UsernameInput from "./UsernameInput.vue";
import type { ChangePasswordPayload, UpdateUserPayload, User } from "@/types/users";
import type { Configuration } from "@/types/configuration";
import type { PasswordSettings, UniqueNameSettings } from "@/types/settings";
import { ErrorCodes, StatusCodes } from "@/types/api";
import { isError } from "@/helpers/error";
import { updateUser } from "@/api/users";
import { useForm } from "@/forms";

const { t } = useI18n();

const props = defineProps<{
  configuration?: Configuration;
  user: User;
}>();

const confirm = ref<string>("");
const password = ref<ChangePasswordPayload>({ new: "" });
const uniqueName = ref<string>("");
const uniqueNameAlreadyUsed = ref<boolean>(false);

const isPasswordRequired = computed<boolean>(() => Boolean(password.value.new || confirm.value));
const passwordSettings = computed<PasswordSettings | undefined>(() => props.user.realm?.passwordSettings ?? props.configuration?.passwordSettings);
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
      password: password.value.new ? password.value : undefined,
      customAttributes: [],
      roles: [],
    };
    const user: User = await updateUser(props.user.id, payload);
    password.value.new = "";
    confirm.value = "";
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
    <h5>{{ t("users.username") }}</h5>
    <UniqueNameAlreadyUsed v-model="uniqueNameAlreadyUsed" />
    <UsernameInput required :settings="uniqueNameSettings" v-model="uniqueName" />
    <h5>{{ t("users.password.label") }}</h5>
    <p v-if="user.passwordChangedBy && user.passwordChangedOn">
      <StatusInfo :actor="user.passwordChangedBy" :date="user.passwordChangedOn" format="users.password.changed" />
    </p>
    <div class="row">
      <PasswordInput class="col" :required="isPasswordRequired" :settings="isPasswordRequired ? passwordSettings : undefined" v-model="password.new" />
      <PasswordInput
        class="col"
        :confirm="password.new"
        id="confirm"
        label="users.password.confirm"
        :required="isPasswordRequired"
        :target="t('users.password.label').toLowerCase()"
        v-model="confirm"
      />
    </div>
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
