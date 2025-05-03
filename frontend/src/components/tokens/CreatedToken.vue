<script setup lang="ts">
import { TarAlert, TarButton, TarInput } from "logitar-vue3-ui";
import { ref } from "vue";
import { useI18n } from "vue-i18n";

const { t } = useI18n();

const props = defineProps<{
  token: string;
}>();

const clipboardRef = ref<InstanceType<typeof TarInput> | null>(null);

function copyToClipboard(): void {
  if (clipboardRef.value) {
    clipboardRef.value.focus();
    navigator.clipboard.writeText(props.token);
  }
}
</script>

<template>
  <div>
    <h3>{{ t("tokens.created.title") }}</h3>
    <TarAlert show variant="success">
      <label for="token" class="fw-bold">{{ t("tokens.created.label") }}</label>
      <TarInput described-by="decoder" id="token" :model-value="token" readonly ref="clipboardRef" @focus="$event.target.select()">
        <template #append>
          <TarButton icon="fas fa-clipboard" :text="t('actions.clipboard')" variant="warning" @click="copyToClipboard()" />
        </template>
      </TarInput>
      <a id="decoder" href="https://jwt.io/" target="_blank">
        {{ t("tokens.created.decoder") }} <font-awesome-icon icon="fas fa-arrow-up-right-from-square" />
      </a>
    </TarAlert>
  </div>
</template>
