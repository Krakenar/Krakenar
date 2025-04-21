<script setup lang="ts">
import { useI18n } from "vue-i18n";

const version = import.meta.env.VITE_APP_VERSION;
const year: number = new Date().getUTCFullYear();
const { t } = useI18n();
</script>

<template>
  <footer class="bg-dark">
    <div class="fluid">
      <RouterLink :to="{ name: 'Home' }" class="text-light">
        <img src="@/assets/img/logo.png" :alt="`${t('brand')} Logo`" height="32" />
      </RouterLink>
      {{ " " }}
      <RouterLink :to="{ name: 'Home' }" class="text-light">
        {{ t("copyright", { version, year }) }}
      </RouterLink>
    </div>
  </footer>
</template>

<style scoped>
footer {
  position: absolute;
  bottom: 0;
  width: 100%;
  height: 60px;
  line-height: 60px;
  text-align: center;
}
</style>
