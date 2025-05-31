<script setup lang="ts">
import { onMounted } from "vue";
import { useRouter } from "vue-router";

const router = useRouter();

onMounted(() => router.push({ name: "Dashboard" }));
</script>

<template>
  <main class="container"></main>
</template>
