<script setup lang="ts">
import { TarButton, TarModal } from "logitar-vue3-ui";
import { computed, ref } from "vue";
import { useI18n } from "vue-i18n";

import CountrySelect from "@/components/users/CountrySelect.vue";
import EmailAddressInput from "@/components/users/EmailAddressInput.vue";
import PhoneNumberInput from "@/components/users/PhoneNumberInput.vue";
import SenderProviderFormSelect from "./SenderProviderFormSelect.vue";
import countries from "@/resources/countries.json";
import type { Country, EmailPayload, PhonePayload } from "@/types/users";
import type { CreateOrReplaceSenderPayload, Sender, SenderKind, SenderProvider } from "@/types/senders";
import { createSender } from "@/api/senders";
import { useForm } from "@/forms";

const { t } = useI18n();

const email = ref<EmailPayload>({ address: "", isVerified: false });
const modalRef = ref<InstanceType<typeof TarModal> | null>(null);
const phone = ref<PhonePayload>({ countryCode: "CA", number: "", isVerified: false });
const provider = ref<SenderProvider | "">("");

const country = computed<Country | undefined>(() => countries.find(({ code }) => code === phone.value.countryCode));
const kind = computed<SenderKind | undefined>(() => {
  switch (provider.value) {
    case "SendGrid":
      return "Email";
    case "Twilio":
      return "Phone";
  }
  return undefined;
});

function hide(): void {
  modalRef.value?.hide();
}

const emit = defineEmits<{
  (e: "created", value: Sender): void;
  (e: "error", value: unknown): void;
}>();

function onCancel(): void {
  onReset();
  hide();
}
function onReset(): void {
  email.value.address = "";
  phone.value.countryCode = "CA";
  phone.value.number = "";
  reset();
}

const { hasChanges, isSubmitting, handleSubmit, reset } = useForm();
async function submit(): Promise<void> {
  try {
    const payload: CreateOrReplaceSenderPayload = {
      email: kind.value === "Email" ? email.value : undefined,
      phone: kind.value === "Phone" ? phone.value : undefined,
      sendGrid: provider.value === "SendGrid" ? { apiKey: "<YOUR_SENDGRID_API_KEY>" } : undefined,
      twilio: provider.value === "Twilio" ? { accountSid: "<YOUR_TWILIO_ACCOUNT_SID>", authenticationToken: "<YOUR_TWILIO_AUTHENTICATION_TOKEN>" } : undefined,
    };
    const sender: Sender = await createSender(payload);
    emit("created", sender);
    onReset();
    hide();
  } catch (e: unknown) {
    emit("error", e);
  }
}
</script>

<template>
  <span>
    <TarButton icon="fas fa-plus" :text="t('actions.create')" variant="success" data-bs-toggle="modal" data-bs-target="#create-sender" />
    <TarModal :close="t('actions.close')" id="create-sender" ref="modalRef" :title="t('senders.create')">
      <form @submit.prevent="handleSubmit(submit)">
        <SenderProviderFormSelect required v-model="provider" />
        <EmailAddressInput v-if="kind === 'Email'" required v-model="email.address" />
        <template v-if="kind === 'Phone'">
          <CountrySelect required v-model="phone.countryCode" />
          <PhoneNumberInput :country="country" required v-model="phone.number" />
        </template>
      </form>
      <template #footer>
        <TarButton icon="fas fa-ban" :text="t('actions.cancel')" variant="secondary" @click="onCancel" />
        <TarButton
          :disabled="isSubmitting || !hasChanges"
          icon="fas fa-plus"
          :loading="isSubmitting"
          :status="t('loading')"
          :text="t('actions.create')"
          type="submit"
          variant="success"
          @click="handleSubmit(submit)"
        />
      </template>
    </TarModal>
  </span>
</template>
