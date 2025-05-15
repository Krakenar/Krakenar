<script setup lang="ts">
import { TarTab, TarTabs } from "logitar-vue3-ui";
import { inject, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import DeleteFieldType from "@/components/fields/DeleteFieldType.vue";
import FieldTypeGeneral from "@/components/fields/FieldTypeGeneral.vue";
import StatusDetail from "@/components/shared/StatusDetail.vue";
import type { Configuration } from "@/types/configuration";
import type { FieldType } from "@/types/fields";
import { StatusCodes, type ApiFailure } from "@/types/api";
import { formatFieldType } from "@/helpers/format";
import { handleErrorKey } from "@/inject/App";
import { readConfiguration } from "@/api/configuration";
import { readFieldType } from "@/api/fields";
import { useToastStore } from "@/stores/toast";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const toasts = useToastStore();
const { t } = useI18n();

const configuration = ref<Configuration>();
const fieldType = ref<FieldType>();

function setMetadata(updated: FieldType): void {
  if (fieldType.value) {
    fieldType.value.version = updated.version;
    fieldType.value.updatedBy = updated.updatedBy;
    fieldType.value.updatedOn = updated.updatedOn;
  }
}

function onDeleted(): void {
  toasts.success("fields.type.deleted");
  router.push({ name: "FieldTypeList" });
}

function onGeneralUpdated(updated: FieldType): void {
  if (fieldType.value) {
    setMetadata(updated);
    fieldType.value.uniqueName = updated.uniqueName;
    fieldType.value.displayName = updated.displayName;
    fieldType.value.description = updated.description;
  }
  toasts.success("fields.type.updated");
}

onMounted(async () => {
  try {
    const id = route.params.id as string;
    fieldType.value = await readFieldType(id);
    if (!fieldType.value.realm) {
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
    <template v-if="fieldType">
      <h1>{{ formatFieldType(fieldType) }}</h1>
      <StatusDetail :aggregate="fieldType" />
      <div class="mb-3">
        <DeleteFieldType :fieldType="fieldType" @deleted="onDeleted" @error="handleError" />
      </div>
      <TarTabs>
        <TarTab active id="general" :title="t('general')">
          <FieldTypeGeneral :configuration="configuration" :field-type="fieldType" @error="handleError" @updated="onGeneralUpdated" />
        </TarTab>
      </TarTabs>
    </template>
  </main>
</template>
