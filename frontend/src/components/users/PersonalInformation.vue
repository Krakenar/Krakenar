<script setup lang="ts">
import { TarButton } from "logitar-vue3-ui";
import { ref, watch } from "vue";
import { useI18n } from "vue-i18n";

import BirthdateInput from "./BirthdateInput.vue";
import GenderSelect from "./GenderSelect.vue";
import LocaleSelect from "@/components/shared/LocaleSelect.vue";
import PersonNameInput from "./PersonNameInput.vue";
import TimeZoneSelect from "@/components/shared/TimeZoneSelect.vue";
import UrlInput from "@/components/shared/UrlInput.vue";
import type { UpdateUserPayload, User } from "@/types/users";
import { updateUser } from "@/api/users";
import { useForm } from "@/forms";

const { t } = useI18n();

const props = defineProps<{
  user: User;
}>();

const birthdate = ref<Date>();
const firstName = ref<string>("");
const gender = ref<string>("");
const lastName = ref<string>("");
const locale = ref<string>("");
const middleName = ref<string>("");
const nickname = ref<string>("");
const picture = ref<string>("");
const profile = ref<string>("");
const timeZone = ref<string>("");
const website = ref<string>("");

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "updated", value: User): void;
}>();

const { hasChanges, isSubmitting, handleSubmit } = useForm();
async function submit(): Promise<void> {
  try {
    const payload: UpdateUserPayload = {
      firstName: (props.user.firstName ?? "") !== firstName.value ? { value: firstName.value } : undefined,
      middleName: (props.user.middleName ?? "") !== middleName.value ? { value: middleName.value } : undefined,
      lastName: (props.user.lastName ?? "") !== lastName.value ? { value: lastName.value } : undefined,
      nickname: (props.user.nickname ?? "") !== nickname.value ? { value: nickname.value } : undefined,
      birthdate: (props.user.birthdate ?? "") !== (birthdate.value?.toISOString() ?? "") ? { value: birthdate.value } : undefined,
      gender: (props.user.gender ?? "") !== gender.value ? { value: gender.value } : undefined,
      locale: (props.user.locale?.code ?? "") !== locale.value ? { value: locale.value } : undefined,
      timeZone: (props.user.timeZone ?? "") !== timeZone.value ? { value: timeZone.value } : undefined,
      picture: (props.user.picture ?? "") !== picture.value ? { value: picture.value } : undefined,
      profile: (props.user.profile ?? "") !== profile.value ? { value: profile.value } : undefined,
      website: (props.user.website ?? "") !== website.value ? { value: website.value } : undefined,
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
  (user) => {
    firstName.value = user.firstName ?? "";
    middleName.value = user.middleName ?? "";
    lastName.value = user.lastName ?? "";
    nickname.value = user.nickname ?? "";
    birthdate.value = user.birthdate ? new Date(user.birthdate) : undefined;
    gender.value = user.gender ?? "";
    locale.value = user.locale?.code ?? "";
    timeZone.value = user.timeZone ?? "";
    picture.value = user.picture ?? "";
    profile.value = user.profile ?? "";
    website.value = user.website ?? "";
  },
  { deep: true, immediate: true },
);
</script>

<template>
  <form @submit.prevent="handleSubmit(submit)">
    <div class="row">
      <PersonNameInput class="col" kind="first" v-model="firstName" />
      <PersonNameInput class="col" kind="last" v-model="lastName" />
    </div>
    <div class="row">
      <PersonNameInput class="col" kind="middle" v-model="middleName" />
      <PersonNameInput class="col" kind="nick" v-model="nickname" />
    </div>
    <div class="row">
      <BirthdateInput class="col" v-model="birthdate" />
      <GenderSelect class="col" v-model="gender" />
    </div>
    <div class="row">
      <LocaleSelect class="col" v-model="locale" />
      <TimeZoneSelect class="col" v-model="timeZone" />
    </div>
    <UrlInput id="picture" :label="t('users.picture')" v-model="picture" />
    <UrlInput id="profile" :label="t('users.profile')" v-model="profile" />
    <UrlInput id="website" :label="t('users.website')" v-model="website" />
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
</template>
