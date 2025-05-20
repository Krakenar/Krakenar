<script setup lang="ts">
import { TarTab, TarTabs } from "logitar-vue3-ui";
import { computed, inject, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import ActiveBadge from "@/components/sessions/ActiveBadge.vue";
import AppBreadcrumb from "@/components/shared/AppBreadcrumb.vue";
import SignOutSession from "@/components/sessions/SignOutSession.vue";
import SignOutUser from "@/components/users/SignOutUser.vue";
import StatusBlock from "@/components/shared/StatusBlock.vue";
import StatusDetail from "@/components/shared/StatusDetail.vue";
import UserAvatar from "@/components/users/UserAvatar.vue";
import type { Breadcrumb } from "@/types/breadcrumb";
import type { CustomAttribute } from "@/types/custom";
import type { Session } from "@/types/sessions";
import type { User } from "@/types/users";
import { StatusCodes, type ApiFailure } from "@/types/api";
import { handleErrorKey } from "@/inject/App";
import { readSession } from "@/api/sessions";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const { t } = useI18n();

const session = ref<Session>();

const additionalInformation = computed<string | undefined>(
  () => session.value?.customAttributes.find((customAttribute) => customAttribute.key === "AdditionalInformation")?.value,
);
const breadcrumb = computed<Breadcrumb[]>(() => [{ route: { name: "SessionList" }, text: t("sessions.title.list") }]);
const customAttributes = computed<CustomAttribute[]>(
  () => session.value?.customAttributes.filter(({ key }) => key !== "AdditionalInformation" && key !== "IpAddress") ?? [],
);
const ipAddress = computed<string | undefined>(() => session.value?.customAttributes.find((customAttribute) => customAttribute.key === "IpAddress")?.value);

function onSessionSignedOut(signedOut: Session): void {
  session.value = signedOut;
}
function onUserSignedOut(user: User): void {
  if (session.value) {
    session.value.user = user;
  }
}

onMounted(async () => {
  try {
    const id = route.params.id as string;
    session.value = await readSession(id);
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
    <h1>{{ t("sessions.title.edit") }}</h1>
    <AppBreadcrumb :current="t('sessions.title.edit')" :parent="breadcrumb" />
    <template v-if="session">
      <StatusDetail :aggregate="session" />
      <div class="mb-3">
        <SignOutSession class="me-1" :session="session" @signed-out="onSessionSignedOut" />
        <SignOutUser class="ms-1" :disabled="!session.isActive" :user="session.user" @signed-out="onUserSignedOut" />
      </div>
      <table class="table table-striped">
        <tbody>
          <tr>
            <th scope="row">{{ t("users.select.label") }}</th>
            <td>
              <UserAvatar target="_blank" :user="session.user" />
            </td>
          </tr>
          <tr>
            <th scope="row">{{ t("sessions.persistent") }}</th>
            <td>
              <template v-if="session.isPersistent"> <font-awesome-icon icon="fas fa-check" /> {{ t("yes") }} </template>
              <template v-else> <font-awesome-icon icon="fas fa-times" /> {{ t("no") }} </template>
            </td>
          </tr>
          <tr>
            <th scope="row">{{ t("sessions.sort.options.SignedOutOn") }}</th>
            <td>
              <StatusBlock v-if="session.signedOutBy && session.signedOutOn" :actor="session.signedOutBy" :date="session.signedOutOn" />
              <ActiveBadge v-else />
            </td>
          </tr>
          <tr v-if="ipAddress">
            <th scope="row">{{ t("sessions.ipAddress") }}</th>
            <td>{{ ipAddress }}</td>
          </tr>
        </tbody>
      </table>
      <TarTabs>
        <TarTab active :title="t('sessions.additionalInformation.title')">
          <json-viewer v-if="additionalInformation" boxed copyable expanded :value="JSON.parse(additionalInformation)" />
          <p v-else>{{ t("sessions.additionalInformation.empty") }}</p>
        </TarTab>
        <TarTab :title="t('customAttributes.label')">
          <table v-if="customAttributes.length > 0" class="table table-striped">
            <thead>
              <tr>
                <th scope="col">{{ t("customAttributes.key") }}</th>
                <th scope="col">{{ t("customAttributes.value") }}</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="customAttribute in customAttributes" :key="customAttribute.key">
                <td>{{ customAttribute.key }}</td>
                <td>{{ customAttribute.value }}</td>
              </tr>
            </tbody>
          </table>
          <p v-else>{{ t("customAttributes.empty") }}</p>
        </TarTab>
      </TarTabs>
    </template>
  </main>
</template>
