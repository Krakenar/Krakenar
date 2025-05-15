<script setup lang="ts">
import { TarButton, TarCheckbox } from "logitar-vue3-ui";
import { useI18n } from "vue-i18n";

import SelectOptionLabel from "./SelectOptionLabel.vue";
import SelectOptionText from "./SelectOptionText.vue";
import SelectOptionValue from "./SelectOptionValue.vue";
import type { SelectOption } from "@/types/fields";

const { t } = useI18n();

const props = withDefaults(
  defineProps<{
    id?: string;
    modelValue: SelectOption;
  }>(),
  {
    id: "edit-option",
  },
);

const emit = defineEmits<{
  (e: "removed"): void;
  (e: "update:model-value", value: SelectOption): void;
}>();

function onDisabledUpdate(isDisabled: boolean): void {
  const option: SelectOption = { ...props.modelValue, isDisabled };
  emit("update:model-value", option);
}
function onLabelUpdate(label: string): void {
  const option: SelectOption = { ...props.modelValue, label };
  emit("update:model-value", option);
}
function onTextUpdate(text: string): void {
  const option: SelectOption = { ...props.modelValue, text };
  emit("update:model-value", option);
}
function onValueUpdate(value: string): void {
  const option: SelectOption = { ...props.modelValue, value };
  emit("update:model-value", option);
}
</script>

<template>
  <div class="row">
    <SelectOptionText class="col" :id="`${id}-text`" :model-value="modelValue.text" required @update:model-value="onTextUpdate">
      <template #append>
        <TarButton icon="fas fa-times" :text="t('actions.remove')" variant="danger" @click="$emit('removed')" />
      </template>
    </SelectOptionText>
    <SelectOptionValue class="col" :id="`${id}-value`" :model-value="modelValue.value ?? ''" @update:model-value="onValueUpdate" />
    <SelectOptionLabel class="col" :id="`${id}-label`" :model-value="modelValue.label ?? ''" @update:model-value="onLabelUpdate">
      <template #append>
        <div class="input-group-text">
          <TarCheckbox
            :id="`${id}-disabled`"
            :label="t('fields.type.select.option.disabled')"
            :model-value="modelValue.isDisabled"
            @update:model-value="onDisabledUpdate"
          />
        </div>
      </template>
    </SelectOptionLabel>
  </div>
</template>
