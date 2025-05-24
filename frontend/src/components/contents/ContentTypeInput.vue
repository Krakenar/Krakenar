<script setup lang="ts">
import { TarInput } from "logitar-vue3-ui";
import { useI18n } from "vue-i18n";

import type { ContentType } from "@/types/contents";
import { formatContentType } from "@/helpers/format";

const { t } = useI18n();

withDefaults(
  defineProps<{
    contentType: ContentType;
    id?: string;
  }>(),
  {
    id: "type",
  },
);
</script>

<template>
  <TarInput class="mb-3" disabled floating :id="id" :label="t('contents.type.select.label')" :model-value="formatContentType(contentType)" />
</template>
