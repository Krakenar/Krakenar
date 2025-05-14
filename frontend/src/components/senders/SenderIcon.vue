<script setup lang="ts">
import { computed } from "vue";

import type { Sender } from "@/types/senders";

const props = defineProps<{
  sender?: Sender;
}>();

const icon = computed<string>(() => {
  switch (props.sender?.kind) {
    case "Email":
      return "fas fa-at";
    case "Phone":
      return "fas fa-phone";
  }
  return "fas fa-paper-plane";
});
</script>

<template>
  <font-awesome-icon :icon="icon" />
</template>
