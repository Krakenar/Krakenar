<script setup lang="ts">
import { TarTab, TarTabs } from "logitar-vue3-ui";
import { inject, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";
import { useRoute } from "vue-router";

import RealmGeneral from "@/components/realms/RealmGeneral.vue";
import RealmSettings from "@/components/realms/RealmSettings.vue";
import StatusDetail from "@/components/shared/StatusDetail.vue";
import type { Realm } from "@/types/realms";
import { handleErrorKey } from "@/inject/App";
import { readRealm } from "@/api/realms";
import { useToastStore } from "@/stores/toast";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const toasts = useToastStore();
const { t } = useI18n();

const realm = ref<Realm>();

function setMetadata(updated: Realm): void {
  if (realm.value) {
    realm.value.version = updated.version;
    realm.value.updatedBy = updated.updatedBy;
    realm.value.updatedOn = updated.updatedOn;
  }
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

onMounted(async () => {
  try {
    const id = route.params.id as string;
    realm.value = await readRealm(id);
  } catch (e: unknown) {
    handleError(e);
  }
});
</script>

<template>
  <main class="container">
    <template v-if="realm">
      <h1>{{ realm.displayName ?? realm.uniqueSlug }}</h1>
      <StatusDetail :aggregate="realm" />
      <TarTabs>
        <TarTab active id="general" :title="t('general')">
          <RealmGeneral :realm="realm" @error="handleError" @updated="onGeneralUpdated" />
        </TarTab>
        <TarTab id="settings" :title="t('settings.title')">
          <RealmSettings :realm="realm" @error="handleError" @updated="onSettingsUpdated" />
        </TarTab>
      </TarTabs>
    </template>
  </main>
</template>
