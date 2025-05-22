<script setup lang="ts">
import { arrayUtils } from "logitar-js";
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import FieldDefinitionEdit from "./FieldDefinitionEdit.vue";
import FieldTypeIcon from "./FieldTypeIcon.vue";
import IndexedBadge from "./IndexedBadge.vue";
import InvariantBadge from "./InvariantBadge.vue";
import RequiredBadge from "./RequiredBadge.vue";
import StatusBlock from "@/components/shared/StatusBlock.vue";
import UniqueBadge from "./UniqueBadge.vue";
import type { ContentType } from "@/types/contents";
import type { FieldDefinition } from "@/types/fields";
import { formatFieldDefinition } from "@/helpers/format";

const { orderBy } = arrayUtils;
const { t } = useI18n();

const props = defineProps<{
  contentType: ContentType;
}>();

const fields = computed<FieldDefinition[]>(() => orderBy(props.contentType.fields, "order"));

defineEmits<{
  (e: "created", value: ContentType): void;
  (e: "error", value: unknown): void;
  (e: "updated", value: ContentType): void;
}>();
</script>

<template>
  <div>
    <div class="mb-3">
      <FieldDefinitionEdit :content-type="contentType" @error="$emit('error', $event)" @saved="$emit('created', $event)" />
    </div>
    <table v-if="fields.length > 0" class="table table-striped">
      <thead>
        <tr>
          <th scope="col">{{ t("fields.definition.names") }}</th>
          <th scope="col">{{ t("fields.type.select.label") }}</th>
          <th scope="col">{{ t("fields.definition.updatedOn") }}</th>
          <th scope="col"></th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="field in fields" :key="field.id">
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
            <FieldDefinitionEdit :content-type="contentType" :field="field" @error="$emit('error', $event)" @saved="$emit('updated', $event)" />
            <!-- TODO(fpion): FieldDefinitionDelete -->
          </td>
        </tr>
      </tbody>
    </table>
    <p v-else>{{ t("fields.definition.empty") }}</p>
  </div>
</template>
