<script setup lang="ts">
import { TarBadge } from "logitar-vue3-ui";
import { useI18n } from "vue-i18n";

const { t } = useI18n();

defineProps<{
  text: string;
}>();
</script>

<template>
  <TarBadge variant="info"> <font-awesome-icon icon="fas fa-certificate" /> {{ t(text) }} </TarBadge>
</template>
