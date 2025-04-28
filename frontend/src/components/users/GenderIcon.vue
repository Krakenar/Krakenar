<script setup lang="ts">
import { computed } from "vue";

const props = defineProps<{
  gender: string;
}>();

const icon = computed<string>(() => {
  const gender: string = props.gender.trim().toLowerCase();
  switch (gender) {
    case "female":
      return "fas fa-venus";
    case "male":
      return "fas fa-mars";
  }
  return "fas fa-genderless";
});
</script>

<template>
  <font-awesome-icon :icon="icon" />
</template>
