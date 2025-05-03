<script setup lang="ts">
import { useI18n } from "vue-i18n";

import FormInput from "@/components/forms/FormInput.vue";

const { t } = useI18n();

withDefaults(
  defineProps<{
    id?: string;
    label?: string;
    modelValue?: string;
  }>(),
  {
    id: "issuer",
    label: "tokens.issuer",
  },
);

defineEmits<{
  (e: "update:model-value", value: string): void;
}>();
</script>

<template>
  <FormInput :id="id" :label="t(label)" :model-value="modelValue" :placeholder="t(label)" @update:model-value="$emit('update:model-value', $event)" />
</template>

<!-- When(x => x.LifetimeSeconds.HasValue, () => RuleFor(x => x.LifetimeSeconds!.Value).GreaterThan(0));
When(x => !string.IsNullOrWhiteSpace(x.Secret), () => RuleFor(x => x.Secret!).Secret());

When(x => x.Email != null, () => RuleFor(x => x.Email!).SetValidator(new EmailValidator()));
RuleForEach(x => x.Claims).SetValidator(new ClaimValidator()); -->
