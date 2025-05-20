<script setup lang="ts">
import { TarTab, TarTabs } from "logitar-vue3-ui";
import { computed, inject, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import AppBreadcrumb from "@/components/shared/AppBreadcrumb.vue";
import CustomAttributeList from "@/components/shared/CustomAttributeList.vue";
import DeleteRole from "@/components/roles/DeleteRole.vue";
import RoleGeneral from "@/components/roles/RoleGeneral.vue";
import StatusDetail from "@/components/shared/StatusDetail.vue";
import type { Breadcrumb } from "@/types/breadcrumb";
import type { Configuration } from "@/types/configuration";
import type { CustomAttribute } from "@/types/custom";
import type { Role, UpdateRolePayload } from "@/types/roles";
import { StatusCodes, type ApiFailure } from "@/types/api";
import { formatRole } from "@/helpers/format";
import { handleErrorKey } from "@/inject/App";
import { readConfiguration } from "@/api/configuration";
import { readRole, updateRole } from "@/api/roles";
import { useToastStore } from "@/stores/toast";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const toasts = useToastStore();
const { t } = useI18n();

const configuration = ref<Configuration>();
const role = ref<Role>();

const breadcrumb = computed<Breadcrumb[]>(() => [{ route: { name: "RoleList" }, text: t("roles.title") }]);
const title = computed<string>(() => (role.value ? formatRole(role.value) : ""));

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

async function saveCustomAttributes(customAttributes: CustomAttribute[]): Promise<void> {
  if (role.value) {
    const payload: UpdateRolePayload = { customAttributes };
    const updated: Role = await updateRole(role.value.id, payload);
    setMetadata(updated);
    role.value.customAttributes = [...updated.customAttributes];
    toasts.success("users.updated");
  }
}

onMounted(async () => {
  try {
    const id = route.params.id as string;
    role.value = await readRole(id);
    if (!role.value.realm) {
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
    <template v-if="role">
      <h1>{{ title }}</h1>
      <AppBreadcrumb :current="title" :parent="breadcrumb" />
      <StatusDetail :aggregate="role" />
      <div class="mb-3">
        <DeleteRole :role="role" @deleted="onDeleted" @error="handleError" />
      </div>
      <TarTabs>
        <TarTab active id="general" :title="t('general')">
          <RoleGeneral :configuration="configuration" :role="role" @error="handleError" @updated="onGeneralUpdated" />
        </TarTab>
        <TarTab id="attributes" :title="t('customAttributes.label')">
          <CustomAttributeList :attributes="role.customAttributes" :save="saveCustomAttributes" @error="handleError" />
        </TarTab>
      </TarTabs>
    </template>
  </main>
</template>
