<script setup lang="ts">
import { TarBadge } from "logitar-vue3-ui";
import { useI18n } from "vue-i18n";

const { t } = useI18n();
</script>

<template>
  <TarBadge variant="info"><font-awesome-icon icon="fas fa-certificate" /> {{ t("users.contact.verified") }}</TarBadge>
</template>
