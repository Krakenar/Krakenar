<script setup lang="ts">
import type { UniqueNameSettings } from "@/types/settings";
import { useI18n } from "vue-i18n";

import AppInput from "@/components/shared/AppInput.vue";

const { t } = useI18n();

const props = defineProps<{
  modelValue: UniqueNameSettings;
}>();

const emit = defineEmits<{
  (e: "update:model-value", value: UniqueNameSettings): void;
}>();

const onAllowedCharactersUpdated = (allowedCharacters: string) => {
  const modelValue: UniqueNameSettings = { ...props.modelValue, allowedCharacters };
  emit("update:model-value", modelValue);
};
</script>

<template>
  <div>
    <h5>{{ t("settings.uniqueName.title") }}</h5>
    <AppInput
      described-by="allowed-characters-help"
      id="allowed-characters"
      :label="t('settings.uniqueName.allowedCharacters.label')"
      :max="255"
      :model-value="modelValue.allowedCharacters ?? ''"
      @update:model-value="onAllowedCharactersUpdated"
    >
      <template #after>
        <div id="allowed-characters-help" class="form-text">{{ t("settings.uniqueName.allowedCharacters.help") }}</div>
      </template>
    </AppInput>
  </div>
</template>
