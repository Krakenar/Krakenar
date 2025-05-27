<script setup lang="ts">
import { TarButton, TarInput } from "logitar-vue3-ui";
import { nanoid } from "nanoid";
import { ref } from "vue";

const props = withDefaults(
  defineProps<{
    id?: string;
    value: string;
  }>(),
  {
    id: () => nanoid(),
  },
);

const isEditing = ref<boolean>(false);
const tag = ref<string>("");

const emit = defineEmits<{
  (e: "removed"): void;
  (e: "updated", value: string): void;
}>();

function onCancel(): void {
  isEditing.value = false;
}
function onEdit(): void {
  tag.value = props.value;
  isEditing.value = true;
}
function onKeyUp(e: KeyboardEvent): void {
  if (e.key === "Escape") {
    onCancel();
  }
}
function onSubmit(): void {
  if (tag.value) {
    emit("updated", tag.value);
    isEditing.value = false;
  }
}
</script>

<template>
  <span>
    <form v-if="isEditing" @keyup="onKeyUp" @submit.prevent="onSubmit">
      <TarInput :id="id" validation="server" v-model="tag">
        <template #append>
          <TarButton icon="fas fa-ban" variant="secondary" @click="onCancel" />
          <TarButton :disabled="!tag" icon="fas fa-save" type="submit" />
        </template>
      </TarInput>
    </form>
    <span v-else class="btn-group" role="group" aria-label="Tag Actions">
      <TarButton :text="value" @click="onEdit" />
      <TarButton icon="fas fa-times" @click="$emit('removed')" />
    </span>
  </span>
</template>
