<script setup lang="ts">
import { TarButton, TarCheckbox } from "logitar-vue3-ui";
import { computed } from "vue";
import { parsingUtils } from "logitar-js";
import { useI18n } from "vue-i18n";

import SelectOptionLabel from "./SelectOptionLabel.vue";
import SelectOptionText from "./SelectOptionText.vue";
import SelectOptionValue from "./SelectOptionValue.vue";
import type { SelectOption } from "@/types/fields";

const { parseBoolean } = parsingUtils;
const { t } = useI18n();

const props = withDefaults(
  defineProps<{
    first?: boolean | string;
    id?: string;
    last?: boolean | string;
    modelValue: SelectOption;
  }>(),
  {
    id: "edit-option",
  },
);

const isFirst = computed<boolean>(() => parseBoolean(props.first) ?? false);
const isLast = computed<boolean>(() => parseBoolean(props.last) ?? false);

const emit = defineEmits<{
  (e: "down"): void;
  (e: "removed"): void;
  (e: "up"): void;
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
    <SelectOptionText class="col-6" :id="`${id}-text`" :model-value="modelValue.text" required @update:model-value="onTextUpdate">
      <template #prepend>
        <TarButton :disabled="isFirst" icon="fas fa-arrow-up" @click="$emit('up')" />
        <TarButton :disabled="isLast" icon="fas fa-arrow-down" @click="$emit('down')" />
      </template>
      <template #append>
        <div class="input-group-text">
          <TarCheckbox
            :id="`${id}-disabled`"
            :label="t('fields.type.select.option.disabled')"
            :model-value="modelValue.isDisabled"
            @update:model-value="onDisabledUpdate"
          />
        </div>
        <TarButton icon="fas fa-times" :text="t('actions.remove')" variant="danger" @click="$emit('removed')" />
      </template>
    </SelectOptionText>
    <SelectOptionValue class="col-3" :id="`${id}-value`" :model-value="modelValue.value ?? ''" @update:model-value="onValueUpdate" />
    <SelectOptionLabel class="col-3" :id="`${id}-label`" :model-value="modelValue.label ?? ''" @update:model-value="onLabelUpdate" />
  </div>
</template>
