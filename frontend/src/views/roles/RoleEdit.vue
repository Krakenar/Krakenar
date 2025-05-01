<script setup lang="ts">
import { TarTab, TarTabs } from "logitar-vue3-ui";
import { inject, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import DeleteRole from "@/components/roles/DeleteRole.vue";
import RoleGeneral from "@/components/roles/RoleGeneral.vue";
import StatusDetail from "@/components/shared/StatusDetail.vue";
import type { Configuration } from "@/types/configuration";
import type { Role } from "@/types/roles";
import { formatRole } from "@/helpers/format";
import { handleErrorKey } from "@/inject/App";
import { readConfiguration } from "@/api/configuration";
import { readRole } from "@/api/roles";
import { useToastStore } from "@/stores/toast";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const toasts = useToastStore();
const { t } = useI18n();

const configuration = ref<Configuration>();
const role = ref<Role>();

function setMetadata(updated: Role): void {
  if (role.value) {
    role.value.version = updated.version;
    role.value.updatedBy = updated.updatedBy;
    role.value.updatedOn = updated.updatedOn;
  }
}

function onDeleted(): void {
  toasts.success("roles.deleted");
  router.push({ name: "RoleList" });
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
    if (!role.value.realm) {
      configuration.value = await readConfiguration();
    }
  } catch (e: unknown) {
    handleError(e);
  }
});
</script>

<template>
  <main class="container">
    <template v-if="role">
      <h1>{{ formatRole(role) }}</h1>
      <StatusDetail :aggregate="role" />
      <div class="mb-3">
        <DeleteRole :role="role" @deleted="onDeleted" @error="handleError" />
      </div>
      <TarTabs>
        <TarTab active id="general" :title="t('general')">
          <RoleGeneral :configuration="configuration" :role="role" @error="handleError" @updated="onGeneralUpdated" />
        </TarTab>
      </TarTabs>
    </template>
  </main>
</template>
