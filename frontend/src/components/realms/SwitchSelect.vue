<script setup lang="ts">
import { TarSelect, type SelectOption } from "logitar-vue3-ui";
import { arrayUtils } from "logitar-js";
import { computed, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";

import type { Realm, SearchRealmsPayload } from "@/types/realms";
import type { SearchResults } from "@/types/search";
import { formatRealm } from "@/helpers/format";
import { searchRealms } from "@/api/realms";
import { useRealmStore } from "@/stores/realm";

const realmStore = useRealmStore();
const { orderBy } = arrayUtils;
const { t } = useI18n();

const realms = ref<Realm[]>([]);

const options = computed<SelectOption[]>(() =>
  orderBy(
    realms.value.map(
      (realm) =>
        ({
          text: formatRealm(realm),
          value: realm.id,
        }) as SelectOption,
    ),
    "text",
  ),
);

const emit = defineEmits<{
  (e: "error", value: unknown): void;
}>();

function onRealmSelected(id: string): void {
  const realm: Realm | undefined = realms.value.find((realm) => realm.id === id);
  if (realm) {
    realmStore.enter(realm);
  } else {
    realmStore.exit();
  }
  window.location.reload();
}

onMounted(async () => {
  try {
    const payload: SearchRealmsPayload = {
      ids: [],
      search: { terms: [], operator: "And" },
      sort: [],
      skip: 0,
      limit: 0,
    };
    const results: SearchResults<Realm> = await searchRealms(payload);
    realms.value = results.items;
  } catch (e: unknown) {
    emit("error", e);
  }
});
</script>

<template>
  <TarSelect
    :disabled="$options.length < 1"
    id="realm-switcher"
    :model-value="realmStore.currentRealm?.id ?? ''"
    :options="options"
    :placeholder="t('realms.none')"
    @update:model-value="onRealmSelected"
  />
</template>
