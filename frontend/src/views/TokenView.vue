<script setup lang="ts">
import { TarTab, TarTabs } from "logitar-vue3-ui";
import { inject } from "vue";
import { useI18n } from "vue-i18n";

import CreateToken from "@/components/tokens/CreateToken.vue";
import ValidateToken from "@/components/tokens/ValidateToken.vue";
import { handleErrorKey } from "@/inject/App";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const { t } = useI18n();
</script>

<template>
  <main class="container">
    <h1>{{ t("tokens.title") }}</h1>
    <TarTabs>
      <TarTab active id="create" :title="t('actions.create')">
        <CreateToken @error="handleError" />
      </TarTab>
      <TarTab id="validate" :title="t('actions.validate')">
        <ValidateToken @error="handleError" />
      </TarTab>
    </TarTabs>
  </main>
</template>
