<script setup lang="ts">
import { TarButton } from "logitar-vue3-ui";
import { computed, ref, watch } from "vue";
import { useI18n } from "vue-i18n";

import DescriptionTextarea from "@/components/shared/DescriptionTextarea.vue";
import DisplayNameInput from "@/components/shared/DisplayNameInput.vue";
import ExpirationInput from "@/components/apiKeys/ExpirationInput.vue";
import type { ApiKey, UpdateApiKeyPayload } from "@/types/apiKeys";
import { isExpired as isApiKeyExpired } from "@/helpers/apiKeys";
import { updateApiKey } from "@/api/apiKeys";
import { useForm } from "@/forms";

const { t } = useI18n();

const props = defineProps<{
  apiKey: ApiKey;
}>();

const description = ref<string>("");
const expiresOn = ref<Date>();
const name = ref<string>("");

const isExpirationRequired = computed<boolean>(() => typeof props.apiKey.expiresOn === "string");
const isExpired = computed<boolean>(() => isApiKeyExpired(props.apiKey));
const maxExpiration = computed<Date>(() => {
  if (typeof props.apiKey.expiresOn === "string") {
    return new Date(props.apiKey.expiresOn);
  }
  const maxExpiration: Date = new Date();
  maxExpiration.setFullYear(maxExpiration.getFullYear() + 100);
  return maxExpiration;
});

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "updated", value: ApiKey): void;
}>();

const { hasChanges, isSubmitting, handleSubmit } = useForm();
async function submit(): Promise<void> {
  try {
    const payload: UpdateApiKeyPayload = {
      name: (props.apiKey.name ?? "") !== name.value ? name.value : undefined,
      expiresOn: !isExpired.value && (props.apiKey.expiresOn ?? "") !== (expiresOn.value?.toISOString() ?? "") ? expiresOn.value : undefined,
      description: (props.apiKey.description ?? "") !== description.value ? { value: description.value } : undefined,
      customAttributes: [],
      roles: [],
    };
    const apiKey: ApiKey = await updateApiKey(props.apiKey.id, payload);
    emit("updated", apiKey);
  } catch (e: unknown) {
    emit("error", e);
  }
}

watch(
  () => props.apiKey,
  (apiKey) => {
    name.value = apiKey.name;
    expiresOn.value = typeof apiKey.expiresOn === "string" ? new Date(apiKey.expiresOn) : undefined;
    description.value = apiKey.description ?? "";
  },
  { deep: true, immediate: true },
);
</script>

<template>
  <div>
    <form @submit.prevent="handleSubmit(submit)">
      <div class="row">
        <DisplayNameInput class="col" v-model="name" />
        <ExpirationInput
          class="col"
          :disabled="isExpired"
          :max="isExpired ? undefined : maxExpiration"
          :required="isExpired ? false : isExpirationRequired"
          v-model="expiresOn"
        />
      </div>
      <DescriptionTextarea v-model="description" />
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
  </div>
</template>
