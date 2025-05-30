<script setup lang="ts">
import { TarButton, TarTab, TarTabs } from "logitar-vue3-ui";
import { computed, inject, onMounted, ref } from "vue";
import { stringUtils } from "logitar-js";
import { useI18n } from "vue-i18n";

import ConfigurationSecret from "@/components/configuration/ConfigurationSecret.vue";
import LoggingSettingsEdit from "@/components/settings/LoggingSettingsEdit.vue";
import PasswordSettingsEdit from "@/components/settings/PasswordSettingsEdit.vue";
import StatusDetail from "@/components/shared/StatusDetail.vue";
import UniqueNameSettingsEdit from "@/components/settings/UniqueNameSettingsEdit.vue";
import type { Configuration, ReplaceConfigurationPayload } from "@/types/configuration";
import type { LoggingSettings, PasswordSettings, UniqueNameSettings } from "@/types/settings";
import { handleErrorKey } from "@/inject/App";
import { readConfiguration, replaceConfiguration } from "@/api/configuration";
import { useForm } from "@/forms";
import { useToastStore } from "@/stores/toast";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const toasts = useToastStore();
const { isNullOrWhiteSpace } = stringUtils;
const { t } = useI18n();

const configuration = ref<Configuration>();
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

const canSubmit = computed<boolean>(
  () => !isSubmitting.value && (hasChanges.value || configuration.value?.loggingSettings.onlyErrors !== loggingSettings.value.onlyErrors),
);

function setModel(model: Configuration): void {
  configuration.value = model;
  loggingSettings.value = { ...model.loggingSettings };
  passwordSettings.value = { ...model.passwordSettings };
  uniqueNameSettings.value = { allowedCharacters: model.uniqueNameSettings.allowedCharacters ?? undefined };
}

const { hasChanges, isSubmitting, handleSubmit } = useForm();
async function submit(): Promise<void> {
  if (configuration.value) {
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
    }
  }
}

function onSecretUpdated(updated: Configuration): void {
  if (configuration.value) {
    configuration.value.version = updated.version;
    configuration.value.updatedBy = updated.updatedBy;
    configuration.value.updatedOn = updated.updatedOn;
    configuration.value.secretChangedBy = updated.secretChangedBy;
    configuration.value.secretChangedOn = updated.secretChangedOn;
  }
  toasts.success("tokens.secret.changed");
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
      <TarTabs>
        <TarTab active id="general" :title="t('general')">
          <form @submit.prevent="handleSubmit(submit)">
            <UniqueNameSettingsEdit v-model="uniqueNameSettings" />
            <PasswordSettingsEdit v-model="passwordSettings" />
            <LoggingSettingsEdit v-model="loggingSettings" />
            <div class="mb-3">
              <TarButton :disabled="!canSubmit" icon="fas fa-save" :loading="isSubmitting" :status="t('loading')" :text="t('actions.save')" type="submit" />
            </div>
          </form>
        </TarTab>
        <TarTab id="secret" :title="t('tokens.secret.label')">
          <ConfigurationSecret :configuration="configuration" @error="handleError" @updated="onSecretUpdated" />
        </TarTab>
      </TarTabs>
    </template>
  </main>
</template>
