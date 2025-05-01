<script setup lang="ts">
import { TarCheckbox } from "logitar-vue3-ui";
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
    verified?: boolean | string;
  }>(),
  {
    id: "address-street",
    label: "users.address.street",
    max: 255,
  },
);

const isVerified = computed<boolean>(() => parseBoolean(props.verified) ?? false);

const emit = defineEmits<{
  (e: "update:model-value", value: string): void;
  (e: "verified", value: boolean): void;
}>();

function onStreetChange(value: string) {
  emit("update:model-value", value);
  emit("verified", false);
}
</script>

<template>
  <FormInput :id="id" :label="t(label)" :max="max" :model-value="modelValue" :required="required" @update:model-value="onStreetChange">
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
