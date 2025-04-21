<script setup lang="ts">
import { TarInput, type InputOptions } from "logitar-vue3-ui";
import { ref } from "vue";

withDefaults(defineProps<InputOptions>(), {
  floating: true,
});

const inputRef = ref<InstanceType<typeof TarInput> | null>(null);

defineEmits<{
  (e: "update:model-value", value: string): void;
}>();

function focus(): void {
  inputRef.value?.focus();
}
defineExpose({ focus });
</script>

<template>
  <TarInput
    class="mb-3"
    :described-by="describedBy"
    :disabled="disabled"
    :floating="floating"
    :id="id"
    :label="label"
    :max="max"
    :min="min"
    :model-value="modelValue"
    :name="name"
    :pattern="pattern"
    :placeholder="placeholder ?? label"
    :plaintext="plaintext"
    :readonly="readonly"
    ref="inputRef"
    :required="required"
    :size="size"
    :status="status"
    :step="step"
    :type="type"
    @update:model-value="$emit('update:model-value', $event)"
  >
    <template #before>
      <slot name="before"></slot>
    </template>
    <template #prepend>
      <slot name="prepend"></slot>
    </template>
    <template #label-override>
      <slot name="label-override"></slot>
    </template>
    <template #label-required>
      <slot name="label-required"></slot>
    </template>
    <template #append>
      <slot name="append"></slot>
    </template>
    <template #after>
      <slot name="after"></slot>
    </template>
  </TarInput>
</template>
