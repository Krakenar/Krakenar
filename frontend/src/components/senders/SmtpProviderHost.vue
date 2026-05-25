<script setup lang="ts">
import type { InputType } from "logitar-vue3-ui";
import { useI18n } from "vue-i18n";

const { t } = useI18n();

import FormInput from "@/components/forms/FormInput.vue";

withDefaults(
  defineProps<{
    id?: string;
    label?: string;
    modelValue?: string;
    required?: boolean | string;
    type?: InputType;
  }>(),
  {
    id: "host",
    label: "senders.smtpProvider.host.label",
    type: "text",
  },
);

defineEmits<{
  (e: "update:model-value", value: string): void;
}>();
</script>

<template>
  <FormInput
    :described-by="`${id}-help`"
    :id="id"
    :label="t(label)"
    :model-value="modelValue"
    :required="required"
    :type="type"
    @update:model-value="$emit('update:model-value', $event)"
  >
    <template #after>
      <div :id="`${id}-help`" class="form-text">
        {{ t("senders.smtpProvider.host.help") }}
        {{ "smtp.example.com" }}
      </div>
    </template>
  </FormInput>
</template>
