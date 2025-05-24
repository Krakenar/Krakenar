<script setup lang="ts">
import { inject, onMounted } from "vue";
import { useRouter } from "vue-router";

import type { CurrentUser } from "@/types/account";
import { handleErrorKey } from "@/inject/App";
import { signOutSession } from "@/api/sessions";
import { useAccountStore } from "@/stores/account";
import { useRealmStore } from "@/stores/realm";

const account = useAccountStore();
const handleError = inject(handleErrorKey) as (e: unknown) => void;
const realm = useRealmStore();
const router = useRouter();

onMounted(async () => {
  const currentUser: CurrentUser | undefined = account.currentUser;
  if (currentUser) {
    try {
      realm.exit();
      await signOutSession(currentUser.sessionId);
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
