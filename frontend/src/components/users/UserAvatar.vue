<script setup lang="ts">
import { TarAvatar } from "logitar-vue3-ui";

import GenderIcon from "./GenderIcon.vue";
import type { User } from "@/types/users";

defineProps<{
  target?: string;
  user: User;
}>();
</script>

<template>
  <RouterLink :to="{ name: 'UserEdit', params: { id: user.id } }" :target="target">
    <TarAvatar :display-name="user.fullName ?? user.uniqueName" :email-address="user.email?.address" :url="user.picture" />
    <GenderIcon v-if="!user.fullName && user.gender" :gender="user.gender" />
    {{ user.fullName ?? user.uniqueName }}
    <template v-if="user.fullName">
      <br />
      <GenderIcon v-if="user.gender" :gender="user.gender" />
      {{ user.uniqueName }}
    </template>
  </RouterLink>
</template>
