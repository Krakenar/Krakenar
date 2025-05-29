<script setup lang="ts">
import { TarSelect, type SelectOption } from "logitar-vue3-ui";
import { arrayUtils } from "logitar-js";
import { computed, onMounted } from "vue";
import { useI18n } from "vue-i18n";

import type { Realm } from "@/types/realms";
import { formatRealm } from "@/helpers/format";
import { useRealmStore } from "@/stores/realm";

const realmStore = useRealmStore();
const { orderBy } = arrayUtils;
const { t } = useI18n();

const options = computed<SelectOption[]>(() =>
  orderBy(
    realmStore.realms.map(
      (realm) =>
        ({
          text: formatRealm(realm),
          value: realm.id,
        }) as SelectOption,
    ),
    "text",
  ),
);

function onRealmSelected(id: string): void {
  const realm: Realm | undefined = realmStore.realms.find((realm) => realm.id === id);
  if (realm) {
    realmStore.enter(realm);
  } else {
    realmStore.exit();
  }
  window.location.reload();
}

onMounted(realmStore.fetchRealms);
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
