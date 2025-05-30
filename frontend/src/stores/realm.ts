import { defineStore } from "pinia";
import { ref } from "vue";

import type { Realm, SearchRealmsPayload } from "@/types/realms";
import type { SearchResults } from "@/types/search";
import { searchRealms } from "@/api/realms";

export const useRealmStore = defineStore(
  "realm",
  () => {
    const currentRealm = ref<Realm>();
    const realms = ref<Realm[]>([]);

    function enter(realm: Realm): void {
      currentRealm.value = realm;
    }
    function exit(): void {
      currentRealm.value = undefined;
    }

    function deleteRealm(realm: Realm): void {
      if (currentRealm.value?.id === realm.id) {
        currentRealm.value = undefined;
      }

      const index: number = realms.value.findIndex((r) => r.id === realm.id);
      if (index >= 0) {
        realms.value.splice(index, 1);
      }
    }
    async function fetchRealms(): Promise<void> {
      const payload: SearchRealmsPayload = {
        ids: [],
        search: { terms: [], operator: "And" },
        sort: [],
        skip: 0,
        limit: 0,
      };
      const results: SearchResults<Realm> = await searchRealms(payload);
      realms.value = results.items;
    }
    function saveRealm(realm: Realm): void {
      if (currentRealm.value?.id === realm.id) {
        currentRealm.value = realm;
      }

      const index: number = realms.value.findIndex((r) => r.id === realm.id);
      if (index < 0) {
        realms.value.push(realm);
      } else {
        realms.value.splice(index, 1, realm);
      }
    }

    return { currentRealm, realms, deleteRealm, enter, exit, fetchRealms, saveRealm };
  },
  {
    persist: {
      pick: ["currentRealm"],
    },
  },
); // TODO(fpion): unit tests
