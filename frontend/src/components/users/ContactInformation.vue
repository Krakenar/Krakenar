<script setup lang="ts">
import { TarButton } from "logitar-vue3-ui";
import { computed, ref, watch } from "vue";
import { stringUtils } from "logitar-js";
import { useI18n } from "vue-i18n";

import AddressLocalityInput from "./AddressLocalityInput.vue";
import AddressStreetInput from "./AddressStreetInput.vue";
import CountrySelect from "./CountrySelect.vue";
import EmailAddressInput from "./EmailAddressInput.vue";
import PhoneExtensionInput from "./PhoneExtensionInput.vue";
import PhoneNumberInput from "./PhoneNumberInput.vue";
import PostalCodeInput from "./PostalCodeInput.vue";
import RegionSelect from "./RegionSelect.vue";
import countries from "@/resources/countries.json";
import type { AddressPayload, Country, EmailPayload, PhonePayload, UpdateUserPayload, User } from "@/types/users";
import { updateUser } from "@/api/users";
import { useForm } from "@/forms";

const { isNullOrWhiteSpace } = stringUtils;
const { t } = useI18n();

const props = defineProps<{
  user: User;
}>();

const address = ref<AddressPayload>({ street: "", locality: "", postalCode: "", region: "", country: "", isVerified: false });
const email = ref<EmailPayload>({ address: "", isVerified: false });
const phone = ref<PhonePayload>({ countryCode: "", number: "", extension: "", isVerified: false });

const addressCountry = computed<Country | undefined>(() => countries.find(({ code }) => code === address.value.country));
const hasAddressChanged = computed<boolean>(
  () =>
    (props.user.address?.street ?? "") !== address.value.street ||
    (props.user.address?.locality ?? "") !== address.value.locality ||
    (props.user.address?.postalCode ?? "") !== address.value.postalCode ||
    (props.user.address?.region ?? "") !== address.value.region ||
    (props.user.address?.country ?? "") !== address.value.country ||
    (props.user.address?.isVerified ?? false) !== address.value.isVerified,
);
const hasChanges = computed<boolean>(() => hasAddressChanged.value || hasEmailChanged.value || hasPhoneChanged.value);
const hasEmailChanged = computed<boolean>(
  () => (props.user.email?.address ?? "") !== email.value.address || (props.user.email?.isVerified ?? false) !== email.value.isVerified,
);
const hasPhoneChanged = computed<boolean>(
  () =>
    (props.user.phone?.countryCode ?? "") !== phone.value.countryCode ||
    (props.user.phone?.number ?? "") !== phone.value.number ||
    (props.user.phone?.extension ?? "") !== phone.value.extension ||
    (props.user.phone?.isVerified ?? false) !== phone.value.isVerified,
);
const isAddressRequired = computed<boolean>(() =>
  Boolean(
    address.value.street || address.value.locality || address.value.postalCode || address.value.region || address.value.country || address.value.isVerified,
  ),
);
const isPhoneRequired = computed<boolean>(() => Boolean(phone.value.countryCode || phone.value.number || phone.value.extension || phone.value.isVerified));
const isPostalCodeRequired = computed<boolean>(() => Boolean(isAddressRequired.value && addressCountry.value?.postalCode));
const isRegionRequired = computed<boolean>(() => Boolean(isAddressRequired.value && (addressCountry.value?.regions?.length ?? 0) > 0));
const phoneCountry = computed<Country | undefined>(() => countries.find(({ code }) => code === phone.value.countryCode));

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "updated", value: User): void;
}>();

function onAddressCountryChange(country: string): void {
  address.value.country = country;
  address.value.region = "";
  address.value.isVerified = false;
}
function onLocalityChange(locality: string): void {
  address.value.locality = locality;
  address.value.isVerified = false;
}
function onPostalCodeChange(postalCode: string): void {
  address.value.postalCode = postalCode;
  address.value.isVerified = false;
}
function onRegionChange(region: string): void {
  address.value.region = region;
  address.value.isVerified = false;
}

function onPhoneCountryChange(country: string): void {
  phone.value.countryCode = country;
  phone.value.isVerified = false;
}
function onPhoneExtensionChange(extension: string): void {
  phone.value.extension = extension;
  phone.value.isVerified = false;
}

const { isSubmitting, handleSubmit } = useForm();
async function submit(): Promise<void> {
  try {
    const payload: UpdateUserPayload = {
      address: hasAddressChanged.value
        ? {
            value: isNullOrWhiteSpace(address.value.street)
              ? null
              : {
                  street: address.value.street,
                  locality: address.value.locality,
                  postalCode: address.value.postalCode || undefined,
                  region: address.value.region || undefined,
                  country: address.value.country,
                  isVerified: address.value.isVerified,
                },
          }
        : undefined,
      email: hasEmailChanged.value ? { value: isNullOrWhiteSpace(email.value.address) ? null : email.value } : undefined,
      phone: hasPhoneChanged.value
        ? {
            value: isNullOrWhiteSpace(phone.value.number)
              ? null
              : {
                  countryCode: phone.value.countryCode,
                  number: phone.value.number,
                  extension: phone.value.extension || undefined,
                  isVerified: phone.value.isVerified,
                },
          }
        : undefined,
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
</script>

<template>
  <form @submit.prevent="handleSubmit(submit)">
    <div class="mb-3">
      <h5>{{ t("users.email.title") }}</h5>
      <EmailAddressInput :required="email.isVerified" :verified="email.isVerified" v-model="email.address" @verified="email.isVerified = $event" />
      <h5>{{ t("users.phone.title") }}</h5>
      <div class="row">
        <CountrySelect class="col" id="phone-country" :model-value="phone.countryCode" :required="isPhoneRequired" @update:model-value="onPhoneCountryChange" />
        <PhoneNumberInput
          class="col"
          :country="phoneCountry"
          :required="isPhoneRequired"
          :verified="phone.isVerified"
          v-model="phone.number"
          @verified="phone.isVerified = $event"
        />
        <PhoneExtensionInput class="col" :model-value="phone.extension" @update:model-value="onPhoneExtensionChange" />
      </div>
      <h5>{{ t("users.address.title") }}</h5>
      <AddressStreetInput :required="isAddressRequired" :verified="address.isVerified" v-model="address.street" @verified="address.isVerified = $event" />
      <div class="row">
        <AddressLocalityInput class="col" :model-value="address.locality" :required="isAddressRequired" @update:model-value="onLocalityChange" />
        <PostalCodeInput
          class="col"
          :country="addressCountry"
          :model-value="address.postalCode"
          :required="isPostalCodeRequired"
          @update:model-value="onPostalCodeChange"
        />
      </div>
      <div class="row">
        <CountrySelect
          class="col"
          id="address-country"
          :model-value="address.country"
          :required="isAddressRequired"
          @update:model-value="onAddressCountryChange"
        />
        <RegionSelect class="col" :country="addressCountry" :model-value="address.region" :required="isRegionRequired" @update:model-value="onRegionChange" />
      </div>
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
