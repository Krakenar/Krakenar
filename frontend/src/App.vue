<script setup lang="ts">
import { RouterView, useRoute, useRouter } from "vue-router";
import { TarToaster } from "logitar-vue3-ui";
import { provide } from "vue";

import AppFooter from "./components/layout/AppFooter.vue";
import AppNavbar from "./components/layout/AppNavbar.vue";
import type { ApiError } from "./types/api";
import { handleErrorKey } from "./inject/App";
import { useAccountStore } from "./stores/account";
import { useToastStore } from "./stores/toast";

const account = useAccountStore();
const route = useRoute();
const router = useRouter();
const toasts = useToastStore();

function handleError(e: unknown): void {
  if (e) {
    const { status } = e as ApiError;
    if (status === 401) {
      account.signOut();
      toasts.warning("toasts.warning.signedOut");
      router.push({ name: "SignIn", query: { redirect: route.fullPath } });
    } else {
      console.error(e);
      toasts.error();
    }
  } else {
    toasts.error();
  }
}
provide(handleErrorKey, handleError);
</script>

<template>
  <AppNavbar />
  <RouterView class="my-3" />
  <AppFooter />
  <TarToaster :toasts="toasts.toasts" @hidden="toasts.remove" />
</template>
