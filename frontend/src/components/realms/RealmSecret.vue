<script setup lang="ts">
import { TarButton } from "logitar-vue3-ui";
import { ref } from "vue";
import { useI18n } from "vue-i18n";

import SecretInput from "@/components/tokens/SecretInput.vue";
import type { Realm, UpdateRealmPayload } from "@/types/realms";
import { updateRealm } from "@/api/realms";
import { useForm } from "@/forms";

const { t } = useI18n();

const props = defineProps<{
  realm: Realm;
}>();

const isLoading = ref<boolean>(false);
const secret = ref<string>("");

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "updated", value: Realm): void;
}>();

const { hasChanges, isSubmitting, handleSubmit } = useForm();
async function submit(): Promise<void> {
  try {
    const payload: UpdateRealmPayload = {
      secret: { value: secret.value },
      customAttributes: [],
    };
    const realm: Realm = await updateRealm(props.realm.id, payload);
    emit("updated", realm);
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
      const payload: UpdateRealmPayload = {
        secret: { value: null },
        customAttributes: [],
      };
      const realm: Realm = await updateRealm(props.realm.id, payload);
      emit("updated", realm);
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
