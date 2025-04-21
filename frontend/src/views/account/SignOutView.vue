<script setup lang="ts">
import { inject, onMounted } from "vue";
import { useRouter } from "vue-router";

import type { CurrentUser } from "@/types/account";
import { handleErrorKey } from "@/inject/App";
import { signOut } from "@/api/sessions";
import { useAccountStore } from "@/stores/account";

const account = useAccountStore();
const handleError = inject(handleErrorKey) as (e: unknown) => void;
const router = useRouter();

onMounted(async () => {
  const currentUser: CurrentUser | undefined = account.currentUser;
  if (currentUser) {
    try {
      await signOut(currentUser.sessionId);
      account.signOut();
    } catch (e: unknown) {
      handleError(e);
    }
  }
  router.push({ name: "SignIn" });
});
</script>

<template>
  <div></div>
</template>
