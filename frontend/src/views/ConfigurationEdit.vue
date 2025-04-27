<script setup lang="ts">
import { TarButton } from "logitar-vue3-ui";
import { inject, onMounted, ref } from "vue";
import { stringUtils } from "logitar-js";
import { useI18n } from "vue-i18n";

import LoggingSettingsEdit from "@/components/settings/LoggingSettingsEdit.vue";
import PasswordSettingsEdit from "@/components/settings/PasswordSettingsEdit.vue";
import StatusDetail from "@/components/shared/StatusDetail.vue";
import UniqueNameSettingsEdit from "@/components/settings/UniqueNameSettingsEdit.vue";
import type { Configuration, ReplaceConfigurationPayload } from "@/types/configuration";
import type { LoggingSettings, PasswordSettings, UniqueNameSettings } from "@/types/settings";
import { handleErrorKey } from "@/inject/App";
import { readConfiguration, replaceConfiguration } from "@/api/configuration";
import { useToastStore } from "@/stores/toast";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const toasts = useToastStore();
const { isNullOrWhiteSpace } = stringUtils;
const { t } = useI18n();

const configuration = ref<Configuration>();
const isLoading = ref<boolean>(false);
const loggingSettings = ref<LoggingSettings>({ extent: "None", onlyErrors: false });
const passwordSettings = ref<PasswordSettings>({
  requiredLength: 8,
  requiredUniqueChars: 8,
  requireNonAlphanumeric: true,
  requireLowercase: true,
  requireUppercase: true,
  requireDigit: true,
  hashingStrategy: "PBKDF2",
});
const uniqueNameSettings = ref<UniqueNameSettings>({});

function setModel(model: Configuration): void {
  configuration.value = model;
  loggingSettings.value = { ...model.loggingSettings };
  passwordSettings.value = { ...model.passwordSettings };
  uniqueNameSettings.value = { allowedCharacters: model.uniqueNameSettings.allowedCharacters ?? undefined };
}

async function submit(): Promise<void> {
  if (!isLoading.value && configuration.value) {
    isLoading.value = true;
    try {
      const payload: ReplaceConfigurationPayload = {
        uniqueNameSettings: {
          allowedCharacters: isNullOrWhiteSpace(uniqueNameSettings.value.allowedCharacters ?? undefined) ? null : uniqueNameSettings.value.allowedCharacters,
        },
        passwordSettings: passwordSettings.value,
        loggingSettings: loggingSettings.value,
      };
      const replacedConfiguration: Configuration = await replaceConfiguration(payload, configuration.value.version);
      setModel(replacedConfiguration);
      toasts.success("configuration.updated");
    } catch (e: unknown) {
      handleError(e);
    } finally {
      isLoading.value = false;
    }
  }
}

onMounted(async () => {
  try {
    const configuration: Configuration = await readConfiguration();
    setModel(configuration);
  } catch (e: unknown) {
    handleError(e);
  }
});
</script>

<template>
  <main class="container">
    <template v-if="configuration">
      <h1>{{ t("configuration.title") }}</h1>
      <StatusDetail :aggregate="configuration" />
      <form @submit.prevent="submit">
        <UniqueNameSettingsEdit v-model="uniqueNameSettings" />
        <PasswordSettingsEdit v-model="passwordSettings" />
        <LoggingSettingsEdit v-model="loggingSettings" />
        <div class="mb-3">
          <TarButton :disabled="isLoading" icon="fas fa-save" :loading="isLoading" :status="t('loading')" :text="t('actions.save')" type="submit" />
        </div>
      </form>
    </template>
  </main>
</template>
