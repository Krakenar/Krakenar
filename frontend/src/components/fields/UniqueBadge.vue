<script setup lang="ts">
import { TarBadge } from "logitar-vue3-ui";
import { useI18n } from "vue-i18n";

const { t } = useI18n();
</script>

<template>
  <TarBadge><font-awesome-icon icon="fas fa-key" /> {{ t("fields.definition.unique.label") }}</TarBadge>
</template>
