<script setup lang="ts">
import { TarButton, TarCheckbox } from "logitar-vue3-ui";
import { computed, ref, watch } from "vue";
import { stringUtils } from "logitar-js";
import { useI18n } from "vue-i18n";

import PasswordSettingsEdit from "@/components/settings/PasswordSettingsEdit.vue";
import UniqueNameSettingsEdit from "@/components/settings/UniqueNameSettingsEdit.vue";
import type { PasswordSettings, UniqueNameSettings } from "@/types/settings";
import type { Realm, UpdateRealmPayload } from "@/types/realms";
import { arePasswordEqual, areUniqueNameEqual } from "@/helpers/settings";
import { updateRealm } from "@/api/realms";
import { useForm } from "@/forms";

const { isNullOrWhiteSpace } = stringUtils;
const { t } = useI18n();

const props = defineProps<{
  realm: Realm;
}>();

const passwordSettings = ref<PasswordSettings>({
  requiredLength: 8,
  requiredUniqueChars: 8,
  requireNonAlphanumeric: true,
  requireLowercase: true,
  requireUppercase: true,
  requireDigit: true,
  hashingStrategy: "PBKDF2",
});
const requireConfirmedAccount = ref<boolean>(true);
const requireUniqueEmail = ref<boolean>(true);
const uniqueNameSettings = ref<UniqueNameSettings>({});

const canSubmit = computed<boolean>(
  () =>
    !isSubmitting.value &&
    (hasChanges.value ||
      props.realm.passwordSettings.requireLowercase !== passwordSettings.value.requireLowercase ||
      props.realm.passwordSettings.requireUppercase !== passwordSettings.value.requireUppercase ||
      props.realm.passwordSettings.requireDigit !== passwordSettings.value.requireDigit ||
      props.realm.passwordSettings.requireNonAlphanumeric !== passwordSettings.value.requireNonAlphanumeric ||
      props.realm.requireUniqueEmail !== requireUniqueEmail.value ||
      props.realm.requireConfirmedAccount !== requireConfirmedAccount.value),
);

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "updated", value: Realm): void;
}>();

const { hasChanges, isSubmitting, handleSubmit } = useForm();
async function submit(): Promise<void> {
  try {
    const payload: UpdateRealmPayload = {
      uniqueNameSettings: !areUniqueNameEqual(props.realm.uniqueNameSettings, uniqueNameSettings.value)
        ? {
            allowedCharacters: isNullOrWhiteSpace(uniqueNameSettings.value.allowedCharacters ?? undefined) ? null : uniqueNameSettings.value.allowedCharacters,
          }
        : undefined,
      passwordSettings: !arePasswordEqual(props.realm.passwordSettings, passwordSettings.value) ? passwordSettings.value : undefined,
      requireUniqueEmail: props.realm.requireUniqueEmail !== requireUniqueEmail.value ? requireUniqueEmail.value : undefined,
      requireConfirmedAccount: props.realm.requireConfirmedAccount !== requireConfirmedAccount.value ? requireConfirmedAccount.value : undefined,
      customAttributes: [],
    };
    const realm: Realm = await updateRealm(props.realm.id, payload);
    emit("updated", realm);
  } catch (e: unknown) {
    emit("error", e);
  }
}

watch(
  () => props.realm,
  (realm) => {
    uniqueNameSettings.value = { ...realm.uniqueNameSettings };
    passwordSettings.value = { ...realm.passwordSettings };
    requireUniqueEmail.value = realm.requireUniqueEmail;
    requireConfirmedAccount.value = realm.requireConfirmedAccount;
  },
  { deep: true, immediate: true },
);
</script>

<template>
  <form @submit.prevent="handleSubmit(submit)">
    <UniqueNameSettingsEdit v-model="uniqueNameSettings" />
    <PasswordSettingsEdit v-model="passwordSettings" />
    <h5>{{ t("settings.users.title") }}</h5>
    <div class="mb-3">
      <TarCheckbox
        described-by="require-unique-email-help"
        id="require-unique-email"
        :label="t('settings.users.requireUniqueEmail.label')"
        v-model="requireUniqueEmail"
      >
        <template #after>
          <div id="require-unique-email-help" class="form-text">{{ t("settings.users.requireUniqueEmail.help") }}</div>
        </template>
      </TarCheckbox>
    </div>
    <div class="mb-3">
      <TarCheckbox
        described-by="require-confirmed-account-help"
        id="require-confirmed-account"
        :label="t('settings.users.requireConfirmedAccount.label')"
        v-model="requireConfirmedAccount"
      >
        <template #after>
          <div id="require-confirmed-account-help" class="form-text">{{ t("settings.users.requireConfirmedAccount.help") }}</div>
        </template>
      </TarCheckbox>
    </div>
    <div class="mb-3">
      <TarButton :disabled="!canSubmit" icon="fas fa-save" :loading="isSubmitting" :status="t('loading')" :text="t('actions.save')" type="submit" />
    </div>
  </form>
</template>
