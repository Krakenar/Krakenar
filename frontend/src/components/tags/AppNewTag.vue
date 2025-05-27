<script setup lang="ts">
import { TarButton, TarInput } from "logitar-vue3-ui";
import { ref } from "vue";

withDefaults(
  defineProps<{
    id?: string;
  }>(),
  {
    id: "new-tag",
  },
);

const tag = ref<string>("");

const emit = defineEmits<{
  (e: "added", value: string): void;
}>();

function onSubmit(): void {
  if (tag.value) {
    emit("added", tag.value);
    tag.value = "";
  }
}
</script>

<template>
  <form @submit.prevent="onSubmit">
    <TarInput :id="id" v-model="tag">
      <template #append>
        <TarButton :disabled="!tag" icon="fas fa-plus" type="submit" variant="success" />
      </template>
    </TarInput>
  </form>
</template>
