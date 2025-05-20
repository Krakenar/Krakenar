<script setup lang="ts">
import { TarButton } from "logitar-vue3-ui";
import { useI18n } from "vue-i18n";

import type { Realm } from "@/types/realms";
import { useRealmStore } from "@/stores/realm";
import { computed } from "vue";

const realmStore = useRealmStore();
const { t } = useI18n();

const props = defineProps<{
  realm: Realm;
}>();

const isCurrent = computed<boolean>(() => props.realm.id === realmStore.currentRealm?.id);

function toggle(): void {
  if (isCurrent.value) {
    realmStore.exit();
  } else {
    realmStore.enter(props.realm);
  }
}
</script>

<template>
  <TarButton
    :icon="`fas fa-door-${isCurrent ? 'closed' : 'open'}`"
    :text="t(`actions.${isCurrent ? 'exit' : 'enter'}`)"
    :variant="isCurrent ? 'warning' : 'primary'"
    @click="toggle"
  />
</template>
