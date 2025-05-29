<script setup lang="ts">
import { TarTab, TarTabs } from "logitar-vue3-ui";
import { computed, inject, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import AppBreadcrumb from "@/components/shared/AppBreadcrumb.vue";
import CustomAttributeList from "@/components/shared/CustomAttributeList.vue";
import DeleteRealm from "@/components/realms/DeleteRealm.vue";
import RealmGeneral from "@/components/realms/RealmGeneral.vue";
import RealmSettings from "@/components/realms/RealmSettings.vue";
import StatusDetail from "@/components/shared/StatusDetail.vue";
import SwitchButton from "@/components/realms/SwitchButton.vue";
import type { CustomAttribute } from "@/types/custom";
import type { Realm, UpdateRealmPayload } from "@/types/realms";
import { StatusCodes, type ApiFailure } from "@/types/api";
import { formatRealm } from "@/helpers/format";
import { handleErrorKey } from "@/inject/App";
import { readRealm, updateRealm } from "@/api/realms";
import { useRealmStore } from "@/stores/realm";
import { useToastStore } from "@/stores/toast";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const realmStore = useRealmStore();
const route = useRoute();
const router = useRouter();
const toasts = useToastStore();
const { t } = useI18n();

const realm = ref<Realm>();

const title = computed<string>(() => (realm.value ? formatRealm(realm.value) : ""));

function onDeleted(): void {
  toasts.success("realms.deleted");
  router.push({ name: "RealmList" });
}

function setMetadata(updated: Realm): void {
  if (realm.value) {
    realm.value.version = updated.version;
    realm.value.updatedBy = updated.updatedBy;
    realm.value.updatedOn = updated.updatedOn;
  }
  realmStore.saveRealm(updated);
}

function onGeneralUpdated(updated: Realm): void {
  if (realm.value) {
    setMetadata(updated);
    realm.value.uniqueSlug = updated.uniqueSlug;
    realm.value.displayName = updated.displayName;
    realm.value.description = updated.description;
    realm.value.url = updated.url;
  }
  toasts.success("realms.updated");
}
function onSettingsUpdated(updated: Realm): void {
  if (realm.value) {
    setMetadata(updated);
    realm.value.uniqueNameSettings = { ...updated.uniqueNameSettings };
    realm.value.passwordSettings = { ...updated.passwordSettings };
    realm.value.requireUniqueEmail = updated.requireUniqueEmail;
    realm.value.requireConfirmedAccount = updated.requireConfirmedAccount;
  }
  toasts.success("realms.updated");
}

async function saveCustomAttributes(customAttributes: CustomAttribute[]): Promise<void> {
  if (realm.value) {
    const payload: UpdateRealmPayload = { customAttributes };
    const updated: Realm = await updateRealm(realm.value.id, payload);
    setMetadata(updated);
    realm.value.customAttributes = [...updated.customAttributes];
    toasts.success("realms.updated");
  }
}

onMounted(async () => {
  try {
    const id = route.params.id as string;
    realm.value = await readRealm(id);
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
    <template v-if="realm">
      <h1>{{ title }}</h1>
      <AppBreadcrumb :current="title" />
      <StatusDetail :aggregate="realm" />
      <div class="mb-3">
        <DeleteRealm class="me-1" :realm="realm" @deleted="onDeleted" @error="handleError" />
        <SwitchButton class="ms-1" :realm="realm" />
      </div>
      <TarTabs>
        <TarTab active id="general" :title="t('general')">
          <RealmGeneral :realm="realm" @error="handleError" @updated="onGeneralUpdated" />
        </TarTab>
        <TarTab id="settings" :title="t('settings.title')">
          <RealmSettings :realm="realm" @error="handleError" @updated="onSettingsUpdated" />
        </TarTab>
        <TarTab id="attributes" :title="t('customAttributes.label')">
          <CustomAttributeList :attributes="realm.customAttributes" :save="saveCustomAttributes" @error="handleError" />
        </TarTab>
      </TarTabs>
    </template>
  </main>
</template>
