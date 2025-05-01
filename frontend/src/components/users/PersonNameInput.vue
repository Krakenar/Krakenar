<script setup lang="ts">
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import FormInput from "@/components/forms/FormInput.vue";
import type { PersonNameKind } from "@/types/users";

const { t } = useI18n();

const props = withDefaults(
  defineProps<{
    kind: PersonNameKind;
    max?: number | string;
    modelValue?: string;
  }>(),
  {
    max: 255,
  },
);

const id = computed<string>(() => {
  return props.kind === "nick" ? "nickname" : `${props.kind}-name`;
});

defineEmits<{
  (e: "update:model-value", value: string): void;
}>();
</script>

<template>
  <FormInput :id="id" :label="t(`users.name.${kind}`)" :max="max" :model-value="modelValue" @update:model-value="$emit('update:model-value', $event)" />
</template>
