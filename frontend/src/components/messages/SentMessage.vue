<script setup lang="ts">
import { TarAlert } from "logitar-vue3-ui";
import { useI18n } from "vue-i18n";

import ViewIcon from "@/components/shared/ViewIcon.vue";

const { t } = useI18n();

defineProps<{
  id: string;
}>();
</script>

<template>
  <TarAlert show variant="success">
    <p>
      <strong>{{ t("messages.sent.lead") }}</strong> {{ t("messages.sent.help") }}
    </p>
    <RouterLink class="btn btn-primary" :to="{ name: 'MessageView', params: { id } }" target="_blank"><ViewIcon /> {{ t("actions.view") }}</RouterLink>
  </TarAlert>
</template>
