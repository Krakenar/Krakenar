<script setup lang="ts">
import { TarButton } from "logitar-vue3-ui";
import { computed, ref } from "vue";
import { useI18n } from "vue-i18n";

import type { UpdateUserPayload, User } from "@/types/users";
import { updateUser } from "@/api/users";

const { t } = useI18n();

const props = defineProps<{
  disabled?: boolean | string;
  user: User;
}>();

const isLoading = ref<boolean>(false);

const icon = computed<string>(() => `fas fa-${props.user.isDisabled ? "lock-open" : "lock"}`);
const text = computed<string>(() => `actions.${props.user.isDisabled ? "enable" : "disable"}`);

const emit = defineEmits<{
  (e: "disabled", value: User): void;
  (e: "enabled", value: User): void;
  (e: "error", value: unknown): void;
}>();

async function onClick(): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    try {
      const payload: UpdateUserPayload = {
        isDisabled: !props.user.isDisabled,
        customAttributes: [],
        roles: [],
      };
      const user: User = await updateUser(props.user.id, payload);
      if (user.isDisabled) {
        emit("disabled", user);
      } else {
        emit("enabled", user);
      }
    } catch (e: unknown) {
      emit("error", e);
    } finally {
      isLoading.value = false;
    }
  }
}
</script>

<template>
  <TarButton :disabled="isLoading || disabled" :icon="icon" :loading="isLoading" :status="t('loading')" :text="t(text)" variant="warning" @click="onClick" />
</template>
