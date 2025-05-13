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
    id: "account-sid",
    label: "senders.twilio.accountSid.label",
    type: "text",
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
        {{ t("senders.twilio.accountSid.help") }}
        {{ "AC" + "0123456789abcdef0123456789abcdef" }}
      </div>
    </template>
  </FormInput>
</template>
