import { defineStore } from "pinia";
import { ref } from "vue";

import type { CurrentUser } from "@/types/account";

export const useAccountStore = defineStore(
  "account",
  () => {
    const currentUser = ref<CurrentUser>();

    function signIn(value: CurrentUser): void {
      currentUser.value = value;
    }
    function signOut(): void {
      currentUser.value = undefined;
    }

    return { currentUser, signIn, signOut };
  },
  {
    persist: true,
  },
);
