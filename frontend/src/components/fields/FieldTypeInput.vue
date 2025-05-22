<script setup lang="ts">
import { TarInput } from "logitar-vue3-ui";
import { useI18n } from "vue-i18n";

import type { FieldType } from "@/types/fields";
import { formatFieldType } from "@/helpers/format";

const { t } = useI18n();

defineProps<{
  fieldType: FieldType;
  id: string;
}>();
</script>

<template>
  <TarInput class="mb-3" disabled floating :id="id" :label="t('fields.type.select.label')" :model-value="formatFieldType(fieldType)" />
</template>
