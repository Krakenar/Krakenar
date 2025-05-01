<script setup lang="ts">
import { TarCheckbox } from "logitar-vue3-ui";
import type { LoggingExtent, LoggingSettings } from "@/types/settings";
import { useI18n } from "vue-i18n";

import LoggingExtentSelect from "./LoggingExtentSelect.vue";

const { t } = useI18n();

const props = defineProps<{
  modelValue: LoggingSettings;
}>();

const emit = defineEmits<{
  (e: "update:model-value", value: LoggingSettings): void;
}>();

function onExtentUpdated(extent: LoggingExtent): void {
  const modelValue: LoggingSettings = { ...props.modelValue, extent };
  if (extent === "None") {
    modelValue.onlyErrors = false;
  }
  emit("update:model-value", modelValue);
}
function onOnlyErrorsUpdated(onlyErrors: boolean): void {
  const modelValue: LoggingSettings = { ...props.modelValue, onlyErrors };
  emit("update:model-value", modelValue);
}
</script>

<template>
  <div>
    <h5>{{ t("settings.logging.title") }}</h5>
    <LoggingExtentSelect :model-value="modelValue.extent" required @update:model-value="onExtentUpdated($event as LoggingExtent)" />
    <TarCheckbox
      v-if="modelValue.extent !== 'None'"
      class="mb-3"
      described-by="only-errors-help"
      id="only-errors"
      :label="t('settings.logging.onlyErrors.label')"
      :model-value="modelValue.onlyErrors"
      @update:model-value="onOnlyErrorsUpdated"
    >
      <template #after>
        <div id="only-errors-help" class="form-text">{{ t("settings.logging.onlyErrors.help") }}</div>
      </template>
    </TarCheckbox>
  </div>
</template>
