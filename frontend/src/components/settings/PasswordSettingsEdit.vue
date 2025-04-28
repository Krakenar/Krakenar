<script setup lang="ts">
import { TarCheckbox } from "logitar-vue3-ui";
import { computed } from "vue";
import { parsingUtils } from "logitar-js";
import { useI18n } from "vue-i18n";

import FormInput from "@/components/forms/FormInput.vue";
import type { PasswordSettings } from "@/types/settings";

const { parseNumber } = parsingUtils;
const { t } = useI18n();

const props = defineProps<{
  modelValue: PasswordSettings;
}>();

const minimumLength = computed<number>(() => {
  let minimumLength: number = 0;
  if (props.modelValue.requireLowercase) {
    minimumLength++;
  }
  if (props.modelValue.requireUppercase) {
    minimumLength++;
  }
  if (props.modelValue.requireDigit) {
    minimumLength++;
  }
  if (props.modelValue.requireNonAlphanumeric) {
    minimumLength++;
  }
  return minimumLength;
});

const emit = defineEmits<{
  (e: "update:model-value", value: PasswordSettings): void;
}>();

function onRequiredLengthUpdated(requiredLength: number): void {
  const modelValue: PasswordSettings = { ...props.modelValue, requiredLength };
  emit("update:model-value", modelValue);
}
function onRequiredUniqueCharsUpdated(requiredUniqueChars: number): void {
  const modelValue: PasswordSettings = { ...props.modelValue, requiredUniqueChars };
  emit("update:model-value", modelValue);
}
function onRequireLowercaseUpdated(requireLowercase: boolean): void {
  const modelValue: PasswordSettings = { ...props.modelValue, requireLowercase };
  emit("update:model-value", modelValue);
}
function onRequireUppercaseUpdated(requireUppercase: boolean): void {
  const modelValue: PasswordSettings = { ...props.modelValue, requireUppercase };
  emit("update:model-value", modelValue);
}
function onRequireDigitUpdated(requireDigit: boolean): void {
  const modelValue: PasswordSettings = { ...props.modelValue, requireDigit };
  emit("update:model-value", modelValue);
}
function onRequireNonAlphanumericUpdated(requireNonAlphanumeric: boolean): void {
  const modelValue: PasswordSettings = { ...props.modelValue, requireNonAlphanumeric };
  emit("update:model-value", modelValue);
}
</script>

<template>
  <div>
    <h5>{{ t("settings.password.title") }}</h5>
    <div class="row">
      <FormInput
        class="col"
        described-by="required-length-help"
        id="required-length"
        :label="t('settings.password.requiredLength.label')"
        :min="minimumLength"
        :model-value="modelValue.requiredLength.toString()"
        type="number"
        @update:model-value="onRequiredLengthUpdated(parseNumber($event) ?? 0)"
      >
        <template #after>
          <div id="required-length-help" class="form-text">{{ t("settings.password.requiredLength.help") }}</div>
        </template>
      </FormInput>
      <FormInput
        class="col"
        id="required-unique-chars"
        :label="t('settings.password.requiredUniqueChars.label')"
        :max="modelValue.requiredLength"
        :model-value="modelValue.requiredUniqueChars.toString()"
        type="number"
        @update:model-value="onRequiredUniqueCharsUpdated(parseNumber($event) ?? 0)"
      >
        <template #after>
          <div id="required-unique-chars-help" class="form-text">{{ t("settings.password.requiredUniqueChars.help") }}</div>
        </template>
      </FormInput>
    </div>
    <div class="mb-3">
      <TarCheckbox
        id="require-lowercase"
        :label="t('settings.password.requireLowercase')"
        :model-value="modelValue.requireLowercase"
        @update:model-value="onRequireLowercaseUpdated"
      />
      <TarCheckbox
        id="require-uppercase"
        :label="t('settings.password.requireUppercase')"
        :model-value="modelValue.requireUppercase"
        @update:model-value="onRequireUppercaseUpdated"
      />
      <TarCheckbox
        id="require-digit"
        :label="t('settings.password.requireDigit')"
        :model-value="modelValue.requireDigit"
        @update:model-value="onRequireDigitUpdated"
      />
      <TarCheckbox
        id="require-non-alphanumeric"
        :label="t('settings.password.requireNonAlphanumeric')"
        :model-value="modelValue.requireNonAlphanumeric"
        @update:model-value="onRequireNonAlphanumericUpdated"
      />
    </div>
  </div>
</template>
