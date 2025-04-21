<script setup lang="ts">
import { useI18n } from "vue-i18n";

const { t } = useI18n();
</script>

<template>
  <main class="container">
    <h1>{{ t("users.signIn.title") }}</h1>
  </main>
</template>
