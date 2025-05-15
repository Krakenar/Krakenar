<script setup lang="ts">
import { TarTab, TarTabs } from "logitar-vue3-ui";
import { computed, inject, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import DateTimeSettingsEdit from "@/components/fields/DateTimeSettingsEdit.vue";
import DeleteFieldType from "@/components/fields/DeleteFieldType.vue";
import FieldTypeGeneral from "@/components/fields/FieldTypeGeneral.vue";
import NumberSettingsEdit from "@/components/fields/NumberSettingsEdit.vue";
import RelatedContentSettingsEdit from "@/components/fields/RelatedContentSettingsEdit.vue";
import RichTextSettingsEdit from "@/components/fields/RichTextSettingsEdit.vue";
import SelectSettingsEdit from "@/components/fields/SelectSettingsEdit.vue";
import StatusDetail from "@/components/shared/StatusDetail.vue";
import StringSettingsEdit from "@/components/fields/StringSettingsEdit.vue";
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

const hasSettings = computed<boolean>(() => Boolean(fieldType.value && fieldType.value.dataType !== "Boolean" && fieldType.value.dataType !== "Tags"));

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

function onSettingsUpdated(updated: FieldType): void {
  if (fieldType.value) {
    setMetadata(updated);
    fieldType.value.boolean = updated.boolean ? { ...updated.boolean } : fieldType.value.boolean;
    fieldType.value.dateTime = updated.dateTime ? { ...updated.dateTime } : fieldType.value.dateTime;
    fieldType.value.number = updated.number ? { ...updated.number } : fieldType.value.number;
    fieldType.value.relatedContent = updated.relatedContent ? { ...updated.relatedContent } : fieldType.value.relatedContent;
    fieldType.value.richText = updated.richText ? { ...updated.richText } : fieldType.value.richText;
    fieldType.value.select = updated.select ? { ...updated.select } : fieldType.value.select;
    fieldType.value.string = updated.string ? { ...updated.string } : fieldType.value.string;
    fieldType.value.tags = updated.tags ? { ...updated.tags } : fieldType.value.tags;
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
        <TarTab v-if="hasSettings" id="settings" :title="t('settings.title')">
          <DateTimeSettingsEdit v-if="fieldType.dateTime" :id="fieldType.id" :settings="fieldType.dateTime" @error="handleError" @updated="onSettingsUpdated" />
          <NumberSettingsEdit v-if="fieldType.number" :id="fieldType.id" :settings="fieldType.number" @error="handleError" @updated="onSettingsUpdated" />
          <RelatedContentSettingsEdit
            v-if="fieldType.relatedContent"
            :id="fieldType.id"
            :settings="fieldType.relatedContent"
            @error="handleError"
            @updated="onSettingsUpdated"
          />
          <RichTextSettingsEdit v-if="fieldType.richText" :id="fieldType.id" :settings="fieldType.richText" @error="handleError" @updated="onSettingsUpdated" />
          <SelectSettingsEdit v-if="fieldType.select" :id="fieldType.id" :settings="fieldType.select" @error="handleError" @updated="onSettingsUpdated" />
          <StringSettingsEdit v-if="fieldType.string" :id="fieldType.id" :settings="fieldType.string" @error="handleError" @updated="onSettingsUpdated" />
        </TarTab>
      </TarTabs>
    </template>
  </main>
</template>
