<script setup lang="ts">
import { TarTextarea, type TextareaOptions } from "logitar-vue3-ui";
import { ref } from "vue";

withDefaults(defineProps<TextareaOptions>(), {
  floating: true,
});

const inputRef = ref<InstanceType<typeof TarTextarea> | null>(null);

defineEmits<{
  (e: "update:model-value", value: string): void;
}>();

function focus(): void {
  inputRef.value?.focus();
}
defineExpose({ focus });
</script>

<template>
  <TarTextarea
    class="mb-3"
    :cols="cols"
    :described-by="describedBy"
    :disabled="disabled"
    :floating="floating"
    :id="id"
    :label="label"
    :max="max"
    :min="min"
    :model-value="modelValue"
    :name="name"
    :placeholder="placeholder ?? label"
    :plaintext="plaintext"
    :readonly="readonly"
    ref="inputRef"
    :required="required"
    :rows="rows"
    :size="size"
    :status="status"
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
  </TarTextarea>
</template>
