<script setup lang="ts">
import type { RouteLocationRaw } from "vue-router";
import { TarAvatar } from "logitar-vue3-ui";
import { computed } from "vue";
import { parsingUtils } from "logitar-js";
import { useI18n } from "vue-i18n";

import type { Actor } from "@/types/actor";

const { d, t } = useI18n();
const { parseNumber } = parsingUtils;

const props = withDefaults(
  defineProps<{
    actor: Actor;
    date: string;
    format: string;
    revision?: number;
    revisionFormat?: string;
  }>(),
  {
    revisionFormat: "status.revision",
  },
);

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
const parsedRevision = computed<number | undefined>(() => parseNumber(props.revision));
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
  <span>
    {{ t(format, { date: d(date, "medium") }) }}
    <RouterLink v-if="route" :to="route" target="_blank">
      <TarAvatar :display-name="displayName" :email-address="actor.emailAddress" :icon="icon" :size="24" :url="actor.pictureUrl" :variant="variant" />
      {{ displayName }}
    </RouterLink>
    <template v-else>
      <TarAvatar :display-name="displayName" :email-address="actor.emailAddress" :icon="icon" :size="24" :url="actor.pictureUrl" :variant="variant" />
      {{ displayName }}
    </template>
    <template v-if="parsedRevision"> {{ t(revisionFormat, { revision: parsedRevision }) }}</template>
  </span>
</template>
