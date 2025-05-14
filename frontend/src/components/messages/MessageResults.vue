<script setup lang="ts">
import { useI18n } from "vue-i18n";

import type { ResultData } from "@/types/messages";
import { computed } from "vue";

const { t } = useI18n();

const props = defineProps<{
  results: ResultData[];
}>();

const value = computed<object>(() => Object.fromEntries(props.results.map(({ key, value }) => [key, value])));
</script>

<template>
  <div>
    <h3>{{ t("messages.results") }}</h3>
    <json-viewer boxed copyable expanded :value="value" />
  </div>
</template>
