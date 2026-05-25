<script setup lang="ts">
import { TarButton } from "logitar-vue3-ui";
import { ref, watch } from "vue";
import { useI18n } from "vue-i18n";

import PasswordInput from "@/components/users/PasswordInput.vue";
import SmtpProviderHost from "@/components/senders/SmtpProviderHost.vue";
import SmtpProviderPort from "@/components/senders/SmtpProviderPort.vue";
import SmtpProviderSecurity from "@/components/senders/SmtpProviderSecurity.vue";
import UsernameInput from "@/components/users/UsernameInput.vue";
import type { Sender, SmtpSecurityMode, UpdateSenderPayload } from "@/types/senders";
import { updateSender } from "@/api/senders";
import { useForm } from "@/forms";

const { t } = useI18n();

const props = defineProps<{
  sender: Sender;
}>();

const host = ref<string>("");
const password = ref<string>("");
const port = ref<number>(0);
const security = ref<SmtpSecurityMode>("None");
const username = ref<string>("");

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "updated", value: Sender): void;
}>();

const { hasChanges, isSubmitting, handleSubmit } = useForm();
async function submit(): Promise<void> {
  try {
    const payload: UpdateSenderPayload = {
      smtpProvider: {
        host: host.value,
        port: port.value,
        security: security.value,
        username: username.value,
        password: password.value,
      },
    };
    const sender: Sender = await updateSender(props.sender.id, payload);
    emit("updated", sender);
  } catch (e: unknown) {
    emit("error", e);
  }
}

watch(
  props.sender,
  (sender) => {
    host.value = sender.smtpProvider?.host ?? "";
    port.value = sender.smtpProvider?.port ?? 0;
    security.value = sender.smtpProvider?.security ?? "None";
    username.value = sender.smtpProvider?.username ?? "";
    password.value = sender.smtpProvider?.password ?? "";
  },
  { deep: true, immediate: true },
);
</script>

<template>
  <form @submit.prevent="handleSubmit(submit)">
    <SmtpProviderHost required v-model="host" />
    <SmtpProviderPort required v-model="port" />
    <SmtpProviderSecurity required v-model="security" />
    <UsernameInput required v-model="username" />
    <PasswordInput required v-model="password" />
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
