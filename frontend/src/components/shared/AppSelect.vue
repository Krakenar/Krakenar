<script setup lang="ts">
import { TarSelect, type SelectOptions } from "logitar-vue3-ui";
import { ref } from "vue";

withDefaults(defineProps<SelectOptions>(), {
  floating: true,
});

const selectRef = ref<InstanceType<typeof TarSelect> | null>(null);

defineEmits<{
  (e: "update:model-value", value: string): void;
}>();

function focus(): void {
  selectRef.value?.focus();
}
defineExpose({ focus });

// TODO(fpion): remove this
</script>

<template>
  <TarSelect
    :aria-label="ariaLabel"
    class="mb-3"
    :described-by="describedBy"
    :disabled="disabled"
    :floating="floating"
    :id="id"
    :label="label"
    :model-value="modelValue"
    :multiple="multiple"
    :name="name"
    :options="options"
    :placeholder="placeholder ?? label"
    ref="selectRef"
    :required="required"
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
  </TarSelect>
</template>
