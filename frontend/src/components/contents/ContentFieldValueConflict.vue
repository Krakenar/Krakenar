<script setup lang="ts">
import { TarAlert } from "logitar-vue3-ui";
import { useI18n } from "vue-i18n";

import ContentIcon from "./ContentIcon.vue";
import type { ApiError } from "@/types/api";
import type { Language } from "@/types/languages";

const { t } = useI18n();

defineProps<{
  language?: Language | null;
  modelValue: ApiError[];
}>();

const emit = defineEmits<{
  (e: "update:model-value", value: ApiError[]): void;
}>();

function onModelValueUpdate(): void {
  emit("update:model-value", []);
}
</script>

<template>
  <TarAlert :close="t('actions.close')" dismissible :model-value="Boolean(modelValue.length)" variant="warning" @update:model-value="onModelValueUpdate">
    <p>
      <strong>{{ t("contents.item.fieldValueConflict.lead") }}</strong>
      <template v-if="language !== undefined">
        <br />
        {{ t("contents.item.fieldValueConflict.language") }}
        <i>{{ language?.locale.displayName ?? t("contents.item.invariant") }}</i>
      </template>
    </p>
    <ul>
      <li v-for="(conflict, index) in modelValue" :key="index">
        {{ t("contents.item.fieldValueConflict.format", { field: conflict.data.Name }) }}
        <RouterLink :to="{ name: 'ContentEdit', params: { id: conflict.data.ConflictId as string } }" target="_blank">
          <ContentIcon /> {{ t("contents.item.fieldValueConflict.go") }}
        </RouterLink>
      </li>
    </ul>
  </TarAlert>
</template>
