<script setup lang="ts">
import { TarTab, TarTabs } from "logitar-vue3-ui";
import { computed, inject, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import AppBreadcrumb from "@/components/shared/AppBreadcrumb.vue";
import AuthenticationInformation from "@/components/users/AuthenticationInformation.vue";
import ContactInformation from "@/components/users/ContactInformation.vue";
import CustomAttributeList from "@/components/shared/CustomAttributeList.vue";
import CustomIdentifierList from "@/components/users/CustomIdentifierList.vue";
import DeleteUser from "@/components/users/DeleteUser.vue";
import PersonalInformation from "@/components/users/PersonalInformation.vue";
import StatusDetail from "@/components/shared/StatusDetail.vue";
import ToggleUserStatus from "@/components/users/ToggleUserStatus.vue";
import UserRoles from "@/components/users/UserRoles.vue";
import UserSummary from "@/components/users/UserSummary.vue";
import type { Breadcrumb } from "@/types/breadcrumb";
import type { Configuration } from "@/types/configuration";
import type { CurrentUser } from "@/types/account";
import type { CustomAttribute } from "@/types/custom";
import type { UpdateUserPayload, User } from "@/types/users";
import { StatusCodes, type ApiFailure } from "@/types/api";
import { formatUser } from "@/helpers/format";
import { handleErrorKey } from "@/inject/App";
import { readConfiguration } from "@/api/configuration";
import { readUser, updateUser } from "@/api/users";
import { useAccountStore } from "@/stores/account";
import { useRealmStore } from "@/stores/realm";
import { useToastStore } from "@/stores/toast";

const account = useAccountStore();
const handleError = inject(handleErrorKey) as (e: unknown) => void;
const realm = useRealmStore();
const route = useRoute();
const router = useRouter();
const toasts = useToastStore();
const { t } = useI18n();

const configuration = ref<Configuration>();
const user = ref<User>();

const breadcrumb = computed<Breadcrumb[]>(() => [{ route: { name: "UserList" }, text: t("users.title") }]);
const isCurrentUser = computed<boolean>(() => Boolean(user.value && user.value.id === account.currentUser?.id));
const title = computed<string>(() => (user.value ? formatUser(user.value) : ""));

function onDeleted(): void {
  toasts.success("users.deleted");
  router.push({ name: "UserList" });
}

function setCurrentUser(user: User): void {
  if (isCurrentUser.value && account.currentUser) {
    const currentUser: CurrentUser = {
      ...account.currentUser,
      displayName: user.fullName ?? user.uniqueName,
      emailAddress: user.email?.address,
      phoneNumber: user.phone?.e164Formatted,
      pictureUrl: user.picture,
    };
    account.signIn(currentUser);
  }
}

function setMetadata(updated: User): void {
  if (user.value) {
    user.value.version = updated.version;
    user.value.updatedBy = updated.updatedBy;
    user.value.updatedOn = updated.updatedOn;
  }
}

function onActivationToggled(updated: User): void {
  if (user.value) {
    setMetadata(updated);
    user.value.disabledBy = updated.disabledBy;
    user.value.disabledOn = updated.disabledOn;
    user.value.isDisabled = updated.isDisabled;
  }
  toasts.success(`users.${updated.isDisabled ? "disabled" : "enabled"}.success`);
}

function onAuthenticationUpdated(updated: User): void {
  if (user.value) {
    setCurrentUser(updated);
    setMetadata(updated);
    user.value.uniqueName = updated.uniqueName;
  }
  toasts.success("users.updated");
}
function onContactUpdated(updated: User): void {
  if (user.value) {
    setCurrentUser(updated);
    setMetadata(updated);
    user.value.address = updated.address;
    user.value.email = updated.email;
    user.value.phone = updated.phone;
  }
  toasts.success("users.updated");
}
function onPersonalUpdated(updated: User): void {
  if (user.value) {
    setCurrentUser(updated);
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
    user.value.customAttributes = [...updated.customAttributes];
    toasts.success("users.updated");
  }
}

onMounted(async () => {
  try {
    let id: string = "";
    switch (route.name) {
      case "Profile":
        realm.exit();
        if (account.currentUser) {
          id = account.currentUser.id;
        }
        break;
      case "UserEdit":
        id = route.params.id as string;
        break;
    }
    user.value = await readUser(id);
    if (!user.value.realm) {
      configuration.value = await readConfiguration();
    }
  } catch (e: unknown) {
    const { status } = e as ApiFailure;
    if (status === StatusCodes.NotFound) {
      router.push("/not-found");
    } else {
      handleError(e);
    }
  }
});
</script>

<template>
  <main class="container">
    <template v-if="user">
      <h1>{{ title }}</h1>
      <AppBreadcrumb :current="title" :parent="breadcrumb" />
      <StatusDetail :aggregate="user" />
      <div class="mb-3">
        <DeleteUser class="me-1" :disabled="isCurrentUser" :user="user" @deleted="onDeleted" @error="handleError" />
        <ToggleUserStatus
          class="ms-1"
          :disabled="isCurrentUser"
          :user="user"
          @disabled="onActivationToggled"
          @enabled="onActivationToggled"
          @error="handleError"
        />
      </div>
      <UserSummary :user="user" />
      <TarTabs>
        <TarTab active id="authentication" :title="t('users.authentication')">
          <AuthenticationInformation :configuration="configuration" :user="user" @error="handleError" @updated="onAuthenticationUpdated" />
        </TarTab>
        <TarTab id="contact" :title="t('users.contact')">
          <ContactInformation :user="user" @error="handleError" @updated="onContactUpdated" />
        </TarTab>
        <TarTab id="personal" :title="t('users.personal')">
          <PersonalInformation :user="user" @error="handleError" @updated="onPersonalUpdated" />
        </TarTab>
        <TarTab id="attributes" :title="t('customAttributes.label')">
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
