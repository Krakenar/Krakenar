<script setup lang="ts">
import { TarAlert } from "logitar-vue3-ui";
import { useI18n } from "vue-i18n";

const { t } = useI18n();

defineProps<{
  modelValue?: boolean | string;
}>();

defineEmits<{
  (e: "update:model-value", value: false): void;
}>();
</script>

<template>
  <TarAlert :close="t('actions.close')" dismissible :model-value="modelValue" variant="warning" @update:model-value="$emit('update:model-value', $event)">
    <strong>{{ t("realms.uniqueSlug.alreadyUsed.lead") }}</strong> {{ t("realms.uniqueSlug.alreadyUsed.help") }}
  </TarAlert>
</template>
