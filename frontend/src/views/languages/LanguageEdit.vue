<script setup lang="ts">
import { inject, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";
import { useRoute } from "vue-router";

import StatusDetail from "@/components/shared/StatusDetail.vue";
import type { Language } from "@/types/languages";
import { handleErrorKey } from "@/inject/App";
import { readLanguage } from "@/api/languages";
import { useToastStore } from "@/stores/toast";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const toasts = useToastStore();
const { t } = useI18n();

const language = ref<Language>();

onMounted(async () => {
  try {
    const id = route.params.id as string;
    language.value = await readLanguage(id);
  } catch (e: unknown) {
    handleError(e);
  }
});
</script>

<template>
  <main class="container">
    <template v-if="language">
      <h1>{{ language }}</h1>
      <StatusDetail :aggregate="language" />
      <!-- TODO(fpion): delete -->
      <!-- TODO(fpion): set default -->
      <!-- TODO(fpion): edit -->
    </template>
  </main>
</template>
