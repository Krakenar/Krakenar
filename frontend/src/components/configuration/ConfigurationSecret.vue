<script setup lang="ts">
import { TarButton } from "logitar-vue3-ui";
import { ref } from "vue";
import { useI18n } from "vue-i18n";

import SecretInput from "@/components/tokens/SecretInput.vue";
import type { Configuration, UpdateConfigurationPayload } from "@/types/configuration";
import { updateConfiguration } from "@/api/configuration";
import { useForm } from "@/forms";
import StatusInfo from "@/components/shared/StatusInfo.vue";

const { t } = useI18n();

defineProps<{
  configuration: Configuration;
}>();

const isLoading = ref<boolean>(false);
const secret = ref<string>("");

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "updated", value: Configuration): void;
}>();

const { hasChanges, isSubmitting, handleSubmit } = useForm();
async function submit(): Promise<void> {
  try {
    const payload: UpdateConfigurationPayload = {
      secret: { value: secret.value },
    };
    const configuration: Configuration = await updateConfiguration(payload);
    emit("updated", configuration);
  } catch (e: unknown) {
    emit("error", e);
  } finally {
    secret.value = "";
  }
}

async function onGenerate(): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    try {
      const payload: UpdateConfigurationPayload = {
        secret: { value: null },
      };
      const configuration: Configuration = await updateConfiguration(payload);
      emit("updated", configuration);
    } catch (e: unknown) {
      emit("error", e);
    } finally {
      secret.value = "";
      isLoading.value = false;
    }
  }
}
</script>

<template>
  <form @submit.prevent="handleSubmit(submit)">
    <p>
      <StatusInfo :actor="configuration.secretChangedBy" :date="configuration.secretChangedOn" format="tokens.secret.format" />
    </p>
    <p class="text-warning">
      <font-awesome-icon icon="fas fa-exclamation-triangle" />
      {{ t("tokens.secret.warning") }}
    </p>
    <SecretInput required v-model="secret" />
    <div class="mb-3">
      <TarButton
        class="me-1"
        :disabled="isSubmitting || !hasChanges"
        icon="fas fa-save"
        :loading="isSubmitting"
        :status="t('loading')"
        :text="t('actions.save')"
        type="submit"
        variant="warning"
      />
      <TarButton
        class="ms-1"
        :disabled="isLoading"
        icon="fas fa-shuffle"
        :loading="isLoading"
        :status="t('loading')"
        :text="t('actions.generate')"
        variant="warning"
        @click="onGenerate"
      />
    </div>
  </form>
</template>
