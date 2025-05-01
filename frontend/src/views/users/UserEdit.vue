<script setup lang="ts">
import { TarTab, TarTabs } from "logitar-vue3-ui";
import { inject, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";
import { useRoute } from "vue-router";

import AuthenticationInformation from "@/components/users/AuthenticationInformation.vue";
import ContactInformation from "@/components/users/ContactInformation.vue";
import CustomAttributeList from "@/components/shared/CustomAttributeList.vue";
import CustomIdentifierList from "@/components/users/CustomIdentifierList.vue";
import PersonalInformation from "@/components/users/PersonalInformation.vue";
import StatusDetail from "@/components/shared/StatusDetail.vue";
import UserRoles from "@/components/users/UserRoles.vue";
import UserSummary from "@/components/users/UserSummary.vue";
import type { Configuration } from "@/types/configuration";
import type { CustomAttribute } from "@/types/custom";
import type { UpdateUserPayload, User } from "@/types/users";
import { formatUser } from "@/helpers/format";
import { handleErrorKey } from "@/inject/App";
import { readConfiguration } from "@/api/configuration";
import { readUser, updateUser } from "@/api/users";
import { useToastStore } from "@/stores/toast";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const toasts = useToastStore();
const { t } = useI18n();

const configuration = ref<Configuration>();
const user = ref<User>();

function setMetadata(updated: User): void {
  if (user.value) {
    user.value.version = updated.version;
    user.value.updatedBy = updated.updatedBy;
    user.value.updatedOn = updated.updatedOn;
  }
}

function onAuthenticationUpdated(updated: User): void {
  if (user.value) {
    setMetadata(updated);
    user.value.uniqueName = updated.uniqueName;
  }
  toasts.success("users.updated");
}
function onContactUpdated(updated: User): void {
  if (user.value) {
    setMetadata(updated);
    user.value.address = updated.address;
    user.value.email = updated.email;
    user.value.phone = updated.phone;
  }
  toasts.success("users.updated");
}
function onPersonalUpdated(updated: User): void {
  if (user.value) {
    setMetadata(updated);
    user.value.firstName = updated.firstName;
    user.value.middleName = updated.middleName;
    user.value.lastName = updated.lastName;
    user.value.fullName = updated.fullName;
    user.value.nickname = updated.nickname;
    user.value.birthdate = updated.birthdate;
    user.value.gender = updated.gender;
    user.value.locale = updated.locale;
    user.value.timeZone = updated.timeZone;
    user.value.picture = updated.picture;
    user.value.profile = updated.profile;
    user.value.website = updated.website;
  }
  toasts.success("users.updated");
}

function onIdentifierRemoved(updated: User): void {
  if (user.value) {
    setMetadata(updated);
    user.value.customIdentifiers = [...updated.customIdentifiers];
  }
  toasts.success("users.identifiers.removed");
}
function onIdentifierSaved(updated: User): void {
  if (user.value) {
    setMetadata(updated);
    user.value.customIdentifiers = [...updated.customIdentifiers];
  }
  toasts.success("users.identifiers.saved");
}

function onRoleAdded(updated: User): void {
  if (user.value) {
    setMetadata(updated);
    user.value.roles = [...updated.roles];
  }
  toasts.success("users.roles.added");
}
function onRoleRemoved(updated: User): void {
  if (user.value) {
    setMetadata(updated);
    user.value.roles = [...updated.roles];
  }
  toasts.success("users.roles.removed");
}

async function saveCustomAttributes(customAttributes: CustomAttribute[]): Promise<void> {
  if (user.value) {
    const payload: UpdateUserPayload = { customAttributes, roles: [] };
    const updated: User = await updateUser(user.value.id, payload);
    setMetadata(updated);
    toasts.success("users.updated");
    user.value.customAttributes = [...updated.customAttributes];
  }
}

onMounted(async () => {
  try {
    const id = route.params.id as string;
    user.value = await readUser(id);
    if (!user.value.realm) {
      configuration.value = await readConfiguration();
    }
  } catch (e: unknown) {
    handleError(e);
  }
});
</script>

<template>
  <main class="container">
    <template v-if="user">
      <h1>{{ formatUser(user) }}</h1>
      <StatusDetail :aggregate="user" />
      <UserSummary :user="user" />
      <TarTabs>
        <TarTab id="authentication" :title="t('users.authentication')">
          <AuthenticationInformation :configuration="configuration" :user="user" @error="handleError" @updated="onAuthenticationUpdated" />
        </TarTab>
        <TarTab id="contact" :title="t('users.contact')">
          <ContactInformation :user="user" @error="handleError" @updated="onContactUpdated" />
        </TarTab>
        <TarTab id="personal" :title="t('users.personal')">
          <PersonalInformation :user="user" @error="handleError" @updated="onPersonalUpdated" />
        </TarTab>
        <TarTab active id="attributes" :title="t('customAttributes.label')">
          <CustomAttributeList :attributes="user.customAttributes" :save="saveCustomAttributes" @error="handleError" />
        </TarTab>
        <TarTab id="identifiers" :title="t('users.identifiers.title')">
          <CustomIdentifierList :user="user" @error="handleError" @removed="onIdentifierRemoved" @saved="onIdentifierSaved" />
        </TarTab>
        <TarTab id="roles" :title="t('roles.title')">
          <UserRoles :user="user" @added="onRoleAdded" @error="handleError" @removed="onRoleRemoved" />
        </TarTab>
      </TarTabs>
    </template>
  </main>
</template>
