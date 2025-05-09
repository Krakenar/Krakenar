<script setup lang="ts">
import { TarAlert, TarButton, TarInput } from "logitar-vue3-ui";
import { ref } from "vue";
import { useI18n } from "vue-i18n";

const { t } = useI18n();

const props = defineProps<{
  value: string;
}>();

const clipboardRef = ref<InstanceType<typeof TarInput> | null>(null);

function copyToClipboard(): void {
  if (clipboardRef.value) {
    clipboardRef.value.focus();
    navigator.clipboard.writeText(props.value);
  }
}
</script>

<template>
  <TarAlert show variant="warning">
    <label for="x-api-key" class="fw-bold">{{ t("apiKeys.xApiKey.lead") }}</label>
    <TarInput described-by="x-api-key-help" id="x-api-key" :model-value="value" readonly ref="clipboardRef" @focus="$event.target.select()">
      <template #append>
        <TarButton icon="fas fa-clipboard" :text="t('actions.clipboard')" variant="warning" @click="copyToClipboard()" />
      </template>
      <template #after>
        <div id="x-api-key-help" class="form-text">{{ t("apiKeys.xApiKey.help") }}</div>
      </template>
    </TarInput>
  </TarAlert>
</template>
