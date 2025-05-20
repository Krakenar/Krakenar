<script setup lang="ts">
import { useI18n } from "vue-i18n";
import { useRoute } from "vue-router";

import type { Breadcrumb } from "@/types/breadcrumb";
import { useRealmStore } from "@/stores/realm";

const realm = useRealmStore();
const route = useRoute();
const { t } = useI18n();

withDefaults(
  defineProps<{
    ariaLabel?: string;
    current?: string;
    divider?: string;
    parent?: Breadcrumb[];
  }>(),
  {
    divider: "›",
    parent: () => [],
  },
);
</script>

<template>
  <nav :aria-label="ariaLabel ? t(ariaLabel) : undefined" :style="divider ? { '--bs-breadcrumb-divider': `'${divider}'` } : undefined">
    <ol class="breadcrumb">
      <li v-if="route.name !== 'RealmList'" class="breadcrumb-item">
        <RouterLink :to="{ name: 'RealmList' }">{{ t("realms.title") }}</RouterLink>
      </li>
      <li v-if="route.name !== 'RealmList' && route.name !== 'RealmEdit'" class="breadcrumb-item">
        <RouterLink v-if="realm.currentRealm" :to="{ name: 'RealmEdit', params: { id: realm.currentRealm.id } }">
          {{ realm.currentRealm.displayName ?? realm.currentRealm.uniqueSlug }}
        </RouterLink>
        <span v-else class="text-muted">{{ t("realms.none") }}</span>
      </li>
      <li v-for="(crumb, index) in parent" :key="index" class="breadcrumb-item">
        <RouterLink :to="crumb.route">{{ crumb.text }}</RouterLink>
      </li>
      <li v-if="current" class="breadcrumb-item active" aria-current="page">{{ current }}</li>
    </ol>
  </nav>
</template>
