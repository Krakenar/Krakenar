<script setup lang="ts">
import type { RouteLocationRaw } from "vue-router";
import { TarAvatar } from "logitar-vue3-ui";
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import type { Actor } from "@/types/actor";

const { d, t } = useI18n();

const props = defineProps<{
  actor: Actor;
  date: string;
}>();

const displayName = computed<string>(() => {
  const { displayName, type } = props.actor;
  return type === "System" ? t("system") : displayName;
});
const icon = computed<string | undefined>(() => {
  switch (props.actor.type) {
    case "ApiKey":
      return "fas fa-key";
    case "System":
      return "fas fa-robot";
    case "User":
      return "fas fa-user";
  }
});
const route = computed<RouteLocationRaw | undefined>(() => {
  switch (props.actor.type) {
    case "ApiKey":
      return { name: "ApiKeyEdit", params: { id: props.actor.id } };
    case "User":
      return { name: "UserEdit", params: { id: props.actor.id } };
  }
  return undefined;
});
const variant = computed<string | undefined>(() => (props.actor.type === "ApiKey" ? "info" : undefined));
</script>

<template>
  <div class="d-flex">
    <div class="d-flex">
      <div class="d-flex align-content-center flex-wrap mx-1">
        <RouterLink v-if="route" :to="route" target="_blank">
          <TarAvatar :display-name="displayName" :email-address="actor.emailAddress" :icon="icon" :url="actor.pictureUrl" :variant="variant" />
        </RouterLink>
        <TarAvatar v-else :display-name="displayName" :email-address="actor.emailAddress" :icon="icon" :url="actor.pictureUrl" :variant="variant" />
      </div>
    </div>
    <div>
      {{ d(date, "medium") }}
      <br />
      {{ t("by") }}
      <RouterLink v-if="route" :to="route" target="_blank">{{ displayName }}</RouterLink>
      <template v-else>{{ displayName }}</template>
    </div>
  </div>
</template>
