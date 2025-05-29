<script setup lang="ts">
import { useI18n } from "vue-i18n";

import type { FieldDefinition } from "@/types/fields";

const { t } = useI18n();

defineProps<{
  field: FieldDefinition;
}>();
</script>

<template>
  <label :for="field.id">
    {{ field.displayName ?? field.uniqueName }}
    <i v-if="field.isRequired" class="text-secondary">({{ t("fields.definition.required.label") }})</i>
  </label>
</template>
