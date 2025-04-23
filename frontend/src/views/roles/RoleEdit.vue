<script setup lang="ts">
import { TarTab, TarTabs } from "logitar-vue3-ui";
import { inject, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";
import { useRoute } from "vue-router";

import RoleGeneral from "@/components/roles/RoleGeneral.vue";
import StatusDetail from "@/components/shared/StatusDetail.vue";
import type { Role } from "@/types/roles";
import { handleErrorKey } from "@/inject/App";
import { readRole } from "@/api/roles";
import { useToastStore } from "@/stores/toast";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const toasts = useToastStore();
const { t } = useI18n();

const role = ref<Role>();

function setMetadata(updated: Role): void {
  if (role.value) {
    role.value.version = updated.version;
    role.value.updatedBy = updated.updatedBy;
    role.value.updatedOn = updated.updatedOn;
  }
}

function onGeneralUpdated(updated: Role): void {
  if (role.value) {
    setMetadata(updated);
    role.value.uniqueName = updated.uniqueName;
    role.value.displayName = updated.displayName;
    role.value.description = updated.description;
  }
  toasts.success("roles.updated");
}

onMounted(async () => {
  try {
    const id = route.params.id as string;
    role.value = await readRole(id);
  } catch (e: unknown) {
    handleError(e);
  }
});
</script>

<template>
  <main class="container">
    <template v-if="role">
      <h1>{{ role.displayName ?? role.uniqueName }}</h1>
      <StatusDetail :aggregate="role" />
      <TarTabs>
        <TarTab active id="general" :title="t('general')">
          <RoleGeneral :role="role" @error="handleError" @updated="onGeneralUpdated" />
        </TarTab>
      </TarTabs>
    </template>
  </main>
</template>
