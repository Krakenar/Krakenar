<script setup lang="ts">
import { TarTab, TarTabs } from "logitar-vue3-ui";
import { computed, inject, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import ApiKeyGeneral from "@/components/apiKeys/ApiKeyGeneral.vue";
import ApiKeyRoles from "@/components/apiKeys/ApiKeyRoles.vue";
import AppBreadcrumb from "@/components/shared/AppBreadcrumb.vue";
import CustomAttributeList from "@/components/shared/CustomAttributeList.vue";
import DeleteApiKey from "@/components/apiKeys/DeleteApiKey.vue";
import StatusDetail from "@/components/shared/StatusDetail.vue";
import XApiKey from "@/components/apiKeys/XApiKey.vue";
import type { ApiKey, UpdateApiKeyPayload } from "@/types/apiKeys";
import type { Breadcrumb } from "@/types/breadcrumb";
import type { CustomAttribute } from "@/types/custom";
import { StatusCodes, type ApiFailure } from "@/types/api";
import { handleErrorKey } from "@/inject/App";
import { readApiKey, updateApiKey } from "@/api/apiKeys";
import { useToastStore } from "@/stores/toast";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const toasts = useToastStore();
const { t } = useI18n();

const apiKey = ref<ApiKey>();
const xApiKey = ref<string>("");

const breadcrumb = computed<Breadcrumb[]>(() => [{ route: { name: "ApiKeyList" }, text: t("apiKeys.title") }]);

function setMetadata(updated: ApiKey): void {
  if (apiKey.value) {
    apiKey.value.version = updated.version;
    apiKey.value.updatedBy = updated.updatedBy;
    apiKey.value.updatedOn = updated.updatedOn;
  }
}

function onDeleted(): void {
  toasts.success("apiKeys.deleted");
  router.push({ name: "ApiKeyList" });
}

function onGeneralUpdated(updated: ApiKey): void {
  if (apiKey.value) {
    setMetadata(updated);
    apiKey.value.name = updated.name;
    apiKey.value.expiresOn = updated.expiresOn;
    apiKey.value.description = updated.description;
  }
  toasts.success("apiKeys.updated");
}

function onRoleAdded(updated: ApiKey): void {
  if (apiKey.value) {
    setMetadata(updated);
    apiKey.value.roles = [...updated.roles];
  }
  toasts.success("apiKeys.roles.added");
}
function onRoleRemoved(updated: ApiKey): void {
  if (apiKey.value) {
    setMetadata(updated);
    apiKey.value.roles = [...updated.roles];
  }
  toasts.success("apiKeys.roles.removed");
}

async function saveCustomAttributes(customAttributes: CustomAttribute[]): Promise<void> {
  if (apiKey.value) {
    const payload: UpdateApiKeyPayload = { customAttributes, roles: [] };
    const updated: ApiKey = await updateApiKey(apiKey.value.id, payload);
    setMetadata(updated);
    apiKey.value.customAttributes = [...updated.customAttributes];
    toasts.success("apiKeys.updated");
  }
}

onMounted(async () => {
  try {
    const id = route.params.id as string;
    apiKey.value = await readApiKey(id);
    xApiKey.value = route.query["x-api-key"]?.toString() ?? "";
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
    <template v-if="apiKey">
      <h1>{{ apiKey.name }}</h1>
      <AppBreadcrumb :current="apiKey.name" :parent="breadcrumb" />
      <StatusDetail :aggregate="apiKey" />
      <XApiKey v-if="xApiKey" :value="xApiKey" />
      <div class="mb-3">
        <DeleteApiKey :apiKey="apiKey" @deleted="onDeleted" @error="handleError" />
      </div>
      <TarTabs>
        <TarTab active id="general" :title="t('general')">
          <ApiKeyGeneral :api-key="apiKey" @error="handleError" @updated="onGeneralUpdated" />
        </TarTab>
        <TarTab id="attributes" :title="t('customAttributes.label')">
          <CustomAttributeList :attributes="apiKey.customAttributes" :save="saveCustomAttributes" @error="handleError" />
        </TarTab>
        <TarTab id="roles" :title="t('roles.title')">
          <ApiKeyRoles :api-key="apiKey" @added="onRoleAdded" @error="handleError" @removed="onRoleRemoved" />
        </TarTab>
      </TarTabs>
    </template>
  </main>
</template>
