<script setup lang="ts">
import { TarButton } from "logitar-vue3-ui";
import { arrayUtils } from "logitar-js";
import { computed, ref } from "vue";
import { useI18n } from "vue-i18n";

import DeleteFieldDefinition from "./DeleteFieldDefinition.vue";
import FieldDefinitionEdit from "./FieldDefinitionEdit.vue";
import FieldTypeIcon from "./FieldTypeIcon.vue";
import IndexedBadge from "./IndexedBadge.vue";
import InvariantBadge from "./InvariantBadge.vue";
import RequiredBadge from "./RequiredBadge.vue";
import StatusBlock from "@/components/shared/StatusBlock.vue";
import UniqueBadge from "./UniqueBadge.vue";
import type { ContentType } from "@/types/contents";
import type { FieldDefinition, SwitchFieldDefinitionsPayload } from "@/types/fields";
import { formatFieldDefinition } from "@/helpers/format";
import { switchFieldDefinitions } from "@/api/fields/definitions";

const { orderBy } = arrayUtils;
const { t } = useI18n();

const props = defineProps<{
  contentType: ContentType;
}>();

const isLoading = ref<boolean>(false);

const fields = computed<FieldDefinition[]>(() => orderBy(props.contentType.fields, "order"));

const emit = defineEmits<{
  (e: "created", value: ContentType): void;
  (e: "deleted", value: ContentType): void;
  (e: "error", value: unknown): void;
  (e: "reordered", value: ContentType): void;
  (e: "updated", value: ContentType): void;
}>();

async function onMove(index: number, direction: -1 | 1): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    try {
      // NOTE(fpion): -1 moves up the field, +1 moves down the field.
      const source: FieldDefinition | undefined = fields.value[index];
      const destination: FieldDefinition | undefined = fields.value[index + direction];
      const payload: SwitchFieldDefinitionsPayload = {
        fields: [source?.id, destination?.id],
      };
      const contentType: ContentType = await switchFieldDefinitions(props.contentType.id, payload);
      emit("reordered", contentType);
    } catch (e: unknown) {
      emit("error", e);
    } finally {
      isLoading.value = false;
    }
  }
}
</script>

<template>
  <div>
    <div class="mb-3">
      <FieldDefinitionEdit :content-type="contentType" @error="$emit('error', $event)" @saved="$emit('created', $event)" />
    </div>
    <table v-if="fields.length > 0" class="table table-striped">
      <thead>
        <tr>
          <th scope="col">{{ t("fields.definition.name") }}</th>
          <th scope="col">{{ t("fields.type.select.label") }}</th>
          <th scope="col">{{ t("fields.definition.updatedOn") }}</th>
          <th scope="col"></th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="(field, index) in fields" :key="field.id">
          <td>
            {{ formatFieldDefinition(field) }}
            <template v-if="field.isInvariant || field.isRequired || field.isIndexed || field.isUnique">
              <br />
              <InvariantBadge v-if="field.isInvariant" class="me-1" />
              <RequiredBadge v-if="field.isRequired" class="me-1" />
              <IndexedBadge v-if="field.isIndexed" class="me-1" />
              <UniqueBadge v-if="field.isUnique" class="me-1" />
            </template>
          </td>
          <td>
            <RouterLink :to="{ name: 'FieldTypeEdit', params: { id: field.fieldType.id } }" target="_blank">
              <FieldTypeIcon /> {{ field.fieldType.displayName ?? field.fieldType.uniqueName }}
            </RouterLink>
          </td>
          <td><StatusBlock :actor="field.updatedBy" :date="field.updatedOn" /></td>
          <td>
            <div class="btn-group me-1" role="group" aria-label="Basic example">
              <TarButton :disabled="index === 0 || isLoading" icon="fas fa-arrow-up" :loading="isLoading" :status="t('loading')" @click="onMove(index, -1)" />
              <TarButton
                :disabled="index === fields.length - 1 || isLoading"
                icon="fas fa-arrow-down"
                :loading="isLoading"
                :status="t('loading')"
                @click="onMove(index, +1)"
              />
            </div>
            <FieldDefinitionEdit class="mx-1" :content-type="contentType" :field="field" @error="$emit('error', $event)" @saved="$emit('updated', $event)" />
            <DeleteFieldDefinition
              class="ms-1"
              :content-type="contentType"
              :field="field"
              @error="$emit('error', $event)"
              @deleted="$emit('deleted', $event)"
            />
          </td>
        </tr>
      </tbody>
    </table>
    <p v-else>{{ t("fields.definition.empty") }}</p>
  </div>
</template>
