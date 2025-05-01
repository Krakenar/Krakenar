<script setup lang="ts">
import { TarCheckbox, type InputType } from "logitar-vue3-ui";
import { computed } from "vue";
import { parsingUtils } from "logitar-js";
import { useI18n } from "vue-i18n";

import FormInput from "@/components/forms/FormInput.vue";

const { parseBoolean } = parsingUtils;
const { t } = useI18n();

const props = withDefaults(
  defineProps<{
    id?: string;
    label?: string;
    max?: number | string;
    modelValue?: string;
    required?: boolean | string;
    type?: InputType;
    verified?: boolean | string;
  }>(),
  {
    id: "email-address",
    label: "users.email.address",
    max: 255,
    type: "email",
  },
);

const isVerified = computed<boolean>(() => parseBoolean(props.verified) ?? false);

const emit = defineEmits<{
  (e: "update:model-value", value: string): void;
  (e: "verified", value: boolean): void;
}>();

function onAddressChange(value: string) {
  emit("update:model-value", value);
  emit("verified", false);
}
</script>

<template>
  <FormInput :id="id" :label="t(label)" :max="max" :model-value="modelValue" :required="required" :type="type" @update:model-value="onAddressChange">
    <template #append>
      <div class="input-group-text">
        <TarCheckbox
          :id="`${id}-verified`"
          :label="t('users.email.verified.label')"
          :model-value="isVerified"
          @update:model-value="$emit('verified', $event)"
        />
      </div>
    </template>
  </FormInput>
</template>
