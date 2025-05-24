<script setup lang="ts">
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import StatusInfo from "@/components/shared/StatusInfo.vue";
import type { ContentLocale } from "@/types/contents";

const { t } = useI18n();

const props = defineProps<{
  locale: ContentLocale;
}>();

const classes = computed<string>(() => (props.locale.isPublished ? "" : "text-muted"));
</script>

<template>
  <span :classes="classes">
    <template v-if="locale.publishedBy && locale.publishedOn">
      <font-awesome-icon class="me-1" icon="fas fa-bullhorn" />
      <StatusInfo :actor="locale.publishedBy" :date="locale.publishedOn" format="contents.item.published.format" />
    </template>
    <template v-else><font-awesome-icon icon="fas fa-mask" /> {{ t("contents.item.unpublished.label") }}</template>
  </span>
</template>
