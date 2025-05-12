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
    id: "api-key",
    label: "senders.sendGrid.apiKey.label",
    type: "password",
  },
);
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
        {{ t("senders.sendGrid.apiKey.help") }}
        {{ ["SG", "A0b1C2d3E4f5G6h7I8j9K0", "L0m1N2o3P4q5R6s7T8u9V0w1X2y3Z4a5B6c7D8e9F0g"].join(".") }}
      </div>
    </template>
  </FormInput>
</template>
