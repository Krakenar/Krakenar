<script setup lang="ts">
import { TarBadge } from "logitar-vue3-ui";
import { useI18n } from "vue-i18n";

const { t } = useI18n();
</script>

<template>
  <TarBadge variant="info"><font-awesome-icon icon="fas fa-hourglass-end" /> {{ t("apiKeys.expired") }}</TarBadge>
</template>
