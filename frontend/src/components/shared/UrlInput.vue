<script setup lang="ts">
import { TarButton, type InputOptions } from "logitar-vue3-ui";
import { stringUtils } from "logitar-js";
import { useI18n } from "vue-i18n";

import FormInput from "@/components/forms/FormInput.vue";

const { isNullOrWhiteSpace } = stringUtils;
const { t } = useI18n();

const props = withDefaults(defineProps<InputOptions>(), {
  floating: true,
  max: 2048,
  type: "url",
});

function go(): void {
  window.open(props.modelValue?.trim(), "_blank");
}

defineEmits<{
  (e: "update:model-value", value: string): void;
}>();
</script>

<template>
  <FormInput v-bind="props" @update:model-value="$emit('update:model-value', $event)">
    <template #before>
      <slot name="before"></slot>
    </template>
    <template #prepend>
      <slot name="prepend"></slot>
    </template>
    <template #label-override>
      <slot name="label-override"></slot>
    </template>
    <template #label-required>
      <slot name="label-required"></slot>
    </template>
    <template #append>
      <slot name="append"></slot>
      <TarButton :disabled="isNullOrWhiteSpace(modelValue)" icon="fas fa-arrow-up-right-from-square" :text="t('actions.go')" variant="info" @click="go" />
    </template>
    <template #after>
      <slot name="after"></slot>
    </template>
  </FormInput>
</template>
