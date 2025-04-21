<script setup lang="ts">
import type { InputType } from "logitar-vue3-ui";
import { ref } from "vue";
import { useI18n } from "vue-i18n";

import AppInput from "@/components/shared/AppInput.vue";

const { t } = useI18n();

withDefaults(
  defineProps<{
    id?: string;
    label?: string;
    modelValue?: string;
    required?: boolean | string;
    type?: InputType;
  }>(),
  {
    id: "password",
    label: "users.password.label",
    type: "password",
    required: true,
  },
);

const inputRef = ref<InstanceType<typeof AppInput> | null>();

// TODO(fpion): validation when settings

defineEmits<{
  (e: "update:model-value", value: string): void;
}>();

function focus(): void {
  inputRef.value?.focus();
}
defineExpose({ focus });
</script>

<template>
  <AppInput
    :id="id"
    :label="t(label)"
    :model-value="modelValue"
    ref="inputRef"
    :required="required"
    :type="type"
    @update:model-value="$emit('update:model-value', $event)"
  />
</template>
