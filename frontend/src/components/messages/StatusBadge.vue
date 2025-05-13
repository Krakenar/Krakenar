<script setup lang="ts">
import { TarBadge, type BadgeVariant } from "logitar-vue3-ui";
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import type { Message } from "@/types/messages";

const { t } = useI18n();

const props = defineProps<{
  message: Message;
}>();

const variant = computed<BadgeVariant>(() => {
  switch (props.message.status) {
    case "Failed":
      return "danger";
    case "Succeeded":
      return "success";
  }
  return "warning";
});
</script>

<template>
  <TarBadge :variant="variant">{{ t(`messages.status.options.${message.status}`) }}</TarBadge>
</template>
