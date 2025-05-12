<script setup lang="ts">
import { TarButton } from "logitar-vue3-ui";
import { computed, ref, watch } from "vue";
import { useI18n } from "vue-i18n";

import CountrySelect from "@/components/users/CountrySelect.vue";
import DescriptionTextarea from "@/components/shared/DescriptionTextarea.vue";
import DisplayNameInput from "@/components/shared/DisplayNameInput.vue";
import EmailAddressInput from "@/components/users/EmailAddressInput.vue";
import PhoneNumberInput from "@/components/users/PhoneNumberInput.vue";
import countries from "@/resources/countries.json";
import type { Country, EmailPayload, PhonePayload } from "@/types/users";
import type { Sender, UpdateSenderPayload } from "@/types/senders";
import { updateSender } from "@/api/senders";
import { useForm } from "@/forms";

const { t } = useI18n();

const props = defineProps<{
  sender: Sender;
}>();

const description = ref<string>("");
const displayName = ref<string>("");
const email = ref<EmailPayload>({ address: "", isVerified: false });
const phone = ref<PhonePayload>({ countryCode: "CA", number: "", isVerified: false });

const country = computed<Country | undefined>(() => countries.find(({ code }) => code === phone.value.countryCode));

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "updated", value: Sender): void;
}>();

const { hasChanges, isSubmitting, handleSubmit } = useForm();
async function submit(): Promise<void> {
  try {
    const payload: UpdateSenderPayload = {
      email: props.sender.kind === "Email" && (props.sender.email?.address ?? "") !== email.value.address ? email.value : undefined,
      phone:
        (props.sender.kind === "Phone" && (props.sender.phone?.countryCode ?? "") !== phone.value.countryCode) ||
        (props.sender.phone?.number ?? "") !== phone.value.number
          ? phone.value
          : undefined,
      displayName: (props.sender.displayName ?? "") !== displayName.value ? { value: displayName.value } : undefined,
      description: (props.sender.description ?? "") !== description.value ? { value: description.value } : undefined,
    };
    const sender: Sender = await updateSender(props.sender.id, payload);
    emit("updated", sender);
  } catch (e: unknown) {
    emit("error", e);
  }
}

watch(
  () => props.sender,
  (sender) => {
    email.value.address = sender.email?.address ?? "";
    phone.value.countryCode = sender.phone?.countryCode ?? "CA";
    phone.value.number = sender.phone?.number ?? "";
    displayName.value = sender.displayName ?? "";
    description.value = sender.description ?? "";
  },
  { deep: true, immediate: true },
);
</script>

<template>
  <div>
    <form @submit.prevent="handleSubmit(submit)">
      <div class="row">
        <EmailAddressInput v-if="sender.kind === 'Email'" class="col" required v-model="email.address" />
        <template v-else-if="sender.kind === 'Phone'">
          <CountrySelect class="col" required v-model="phone.countryCode" />
          <PhoneNumberInput class="col" :country="country" required v-model="phone.number" />
        </template>
        <DisplayNameInput class="col" v-model="displayName" />
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
