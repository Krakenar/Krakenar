<script setup lang="ts">
import { TarAvatar } from "logitar-vue3-ui";

import type { User } from "@/types/users";

defineProps<{
  user: User;
}>();
</script>

<template>
  <RouterLink :to="{ name: 'UserEdit', params: { id: user.id } }" target="_blank">
    <TarAvatar :display-name="user.fullName ?? user.uniqueName" :email-address="user.email?.address" :url="user.picture" />
    {{ user.fullName ?? user.uniqueName }}
    <template v-if="user.fullName">
      <br />
      {{ user.uniqueName }}
    </template>
  </RouterLink>
</template>
