<script setup lang="ts">
import { useI18n } from "vue-i18n";

import type { Variable } from "@/types/messages";
import { computed } from "vue";

const { t } = useI18n();

const props = defineProps<{
  variables: Variable[];
}>();

const value = computed<object>(() => Object.fromEntries(props.variables.map(({ key, value }) => [key, value])));
</script>

<template>
  <div>
    <h3>{{ t("messages.variables.title") }}</h3>
    <json-viewer boxed copyable expanded :value="value" />
  </div>
</template>
