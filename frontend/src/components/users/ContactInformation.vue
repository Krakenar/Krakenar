<script setup lang="ts">
import { TarButton } from "logitar-vue3-ui";
import { computed, ref, watch } from "vue";
import { useI18n } from "vue-i18n";

import type { AddressPayload, EmailPayload, PhonePayload, UpdateUserPayload, User } from "@/types/users";
import { updateUser } from "@/api/users";
import { useForm } from "@/forms";

const { t } = useI18n();

const props = defineProps<{
  user: User;
}>();

const address = ref<AddressPayload>({ street: "", locality: "", postalCode: "", region: "", country: "", isVerified: false });
const email = ref<EmailPayload>({ address: "", isVerified: false });
const phone = ref<PhonePayload>({ countryCode: "", number: "", extension: "", isVerified: false });

const hasAddressChanged = computed<boolean>(
  () =>
    (props.user.address?.street ?? "") !== address.value.street ||
    (props.user.address?.locality ?? "") !== address.value.locality ||
    (props.user.address?.postalCode ?? "") !== address.value.postalCode ||
    (props.user.address?.region ?? "") !== address.value.region ||
    (props.user.address?.country ?? "") !== address.value.country,
);
const hasEmailChanged = computed<boolean>(() => (props.user.email?.address ?? "") !== email.value.address);
const hasPhoneChanged = computed<boolean>(
  () =>
    (props.user.phone?.countryCode ?? "") !== phone.value.countryCode ||
    (props.user.phone?.number ?? "") !== phone.value.number ||
    (props.user.phone?.extension ?? "") !== phone.value.extension,
);

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "updated", value: User): void;
}>();

const { hasChanges, isSubmitting, handleSubmit } = useForm();
async function submit(): Promise<void> {
  try {
    const payload: UpdateUserPayload = {
      // TODO(fpion): address
      // TODO(fpion): email
      // TODO(fpion): phone
      customAttributes: [],
      roles: [],
    };
    const user: User = await updateUser(props.user.id, payload);
    emit("updated", user);
  } catch (e: unknown) {
    emit("error", e);
  }
}

watch(
  () => props.user,
  () => {
    address.value.street = props.user.address?.street ?? "";
    address.value.locality = props.user.address?.locality ?? "";
    address.value.postalCode = props.user.address?.postalCode ?? "";
    address.value.region = props.user.address?.region ?? "";
    address.value.country = props.user.address?.country ?? "";
    address.value.isVerified = props.user.address?.isVerified ?? false;
    email.value.address = props.user.email?.address ?? "";
    email.value.isVerified = props.user.email?.isVerified ?? false;
    phone.value.countryCode = props.user.phone?.countryCode ?? "";
    phone.value.number = props.user.phone?.number ?? "";
    phone.value.extension = props.user.phone?.extension ?? "";
    phone.value.isVerified = props.user.phone?.isVerified ?? false;
  },
  { deep: true, immediate: true },
);

// TODO(fpion): unverify address when changed
// TODO(fpion): unverify email when changed
// TODO(fpion): unverify phone when changed
</script>

<template>
  <form @submit.prevent="handleSubmit(submit)">
    <div class="mb-3">
      <!-- TODO(fpion): email -->
      <!-- TODO(fpion): phone -->
      <!-- TODO(fpion): address -->
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
