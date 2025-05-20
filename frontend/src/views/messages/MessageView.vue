<script setup lang="ts">
import { TarTab, TarTabs } from "logitar-vue3-ui";
import { computed, inject, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import AppBreadcrumb from "@/components/shared/AppBreadcrumb.vue";
import MessageContents from "@/components/messages/MessageContents.vue";
import MessageGeneral from "@/components/messages/MessageGeneral.vue";
import MessageRecipients from "@/components/messages/MessageRecipients.vue";
import StatusDetail from "@/components/shared/StatusDetail.vue";
import type { Breadcrumb } from "@/types/breadcrumb";
import type { Message } from "@/types/messages";
import { StatusCodes, type ApiFailure } from "@/types/api";
import { handleErrorKey } from "@/inject/App";
import { readMessage } from "@/api/messages";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const { t } = useI18n();

const message = ref<Message>();

const breadcrumb = computed<Breadcrumb[]>(() => [{ route: { name: "MessageList" }, text: t("messages.title") }]);

onMounted(async () => {
  try {
    const id = route.params.id as string;
    message.value = await readMessage(id);
  } catch (e: unknown) {
    const { status } = e as ApiFailure;
    if (status === StatusCodes.NotFound) {
      router.push("/not-found");
    } else {
      handleError(e);
    }
  }
});
</script>

<template>
  <main class="container">
    <template v-if="message">
      <h1>{{ message.subject }}</h1>
      <AppBreadcrumb :current="message.subject" :parent="breadcrumb" />
      <StatusDetail :aggregate="message" />
      <TarTabs>
        <TarTab active id="general" :title="t('general')">
          <MessageGeneral :message="message" />
        </TarTab>
        <TarTab id="contents" :title="t('templates.content')">
          <MessageContents :message="message" />
        </TarTab>
        <TarTab id="recipients" :title="t('messages.recipients.title')">
          <MessageRecipients :message="message" />
        </TarTab>
      </TarTabs>
    </template>
  </main>
</template>
