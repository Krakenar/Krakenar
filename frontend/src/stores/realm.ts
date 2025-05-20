import { defineStore } from "pinia";
import { ref } from "vue";

import type { Realm } from "@/types/realms";

export const useRealmStore = defineStore(
  "realm",
  () => {
    const currentRealm = ref<Realm>();

    function enter(realm: Realm): void {
      currentRealm.value = realm;
    }
    function exit(): void {
      currentRealm.value = undefined;
    }

    return { currentRealm, enter, exit };
  },
  {
    persist: true,
  },
);
