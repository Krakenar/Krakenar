<script setup lang="ts">
import { TarTab, TarTabs } from "logitar-vue3-ui";
import { computed, inject, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import AppBreadcrumb from "@/components/shared/AppBreadcrumb.vue";
import DefaultButton from "@/components/senders/DefaultButton.vue";
import DeleteSender from "@/components/senders/DeleteSender.vue";
import SendDemo from "@/components/messages/SendDemo.vue";
import SendGridSettingsEdit from "@/components/senders/SendGridSettingsEdit.vue";
import SenderGeneral from "@/components/senders/SenderGeneral.vue";
import StatusDetail from "@/components/shared/StatusDetail.vue";
import TwilioSettingsEdit from "@/components/senders/TwilioSettingsEdit.vue";
import type { Breadcrumb } from "@/types/breadcrumb";
import type { Sender } from "@/types/senders";
import { StatusCodes, type ApiFailure } from "@/types/api";
import { formatSender } from "@/helpers/format";
import { handleErrorKey } from "@/inject/App";
import { readSender } from "@/api/senders";
import { useToastStore } from "@/stores/toast";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const toasts = useToastStore();
const { t } = useI18n();

const sender = ref<Sender>();

const breadcrumb = computed<Breadcrumb[]>(() => [{ route: { name: "SenderList" }, text: t("senders.title") }]);
const title = computed<string>(() => (sender.value ? formatSender(sender.value) : ""));

function setMetadata(updated: Sender): void {
  if (sender.value) {
    sender.value.version = updated.version;
    sender.value.updatedBy = updated.updatedBy;
    sender.value.updatedOn = updated.updatedOn;
  }
}

function onDeleted(): void {
  toasts.success("senders.deleted");
  router.push({ name: "SenderList" });
}

function onSetDefault(updated: Sender): void {
  if (sender.value) {
    sender.value.version = updated.version;
    sender.value.updatedBy = updated.updatedBy;
    sender.value.updatedOn = updated.updatedOn;
    sender.value.isDefault = updated.isDefault;
  }
  toasts.success("senders.default.set");
}

function onGeneralUpdated(updated: Sender): void {
  if (sender.value) {
    setMetadata(updated);
    sender.value.email = updated.email ? { ...updated.email } : undefined;
    sender.value.phone = updated.phone ? { ...updated.phone } : undefined;
    sender.value.displayName = updated.displayName;
    sender.value.description = updated.description;
  }
  toasts.success("senders.updated");
}

function onSendGridUpdated(updated: Sender): void {
  if (sender.value) {
    setMetadata(updated);
    if (updated.sendGrid) {
      sender.value.sendGrid = { ...updated.sendGrid };
    }
  }
  toasts.success("senders.updated");
}
function onTwilioUpdated(updated: Sender): void {
  if (sender.value) {
    setMetadata(updated);
    if (updated.twilio) {
      sender.value.twilio = { ...updated.twilio };
    }
  }
  toasts.success("senders.updated");
}

onMounted(async () => {
  try {
    const id = route.params.id as string;
    sender.value = await readSender(id);
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
    <template v-if="sender">
      <h1>{{ title }}</h1>
      <AppBreadcrumb :current="title" :parent="breadcrumb" />
      <StatusDetail :aggregate="sender" />
      <div class="mb-3">
        <DeleteSender class="me-1" :sender="sender" @deleted="onDeleted" @error="handleError" />
        <DefaultButton class="ms-1" :sender="sender" @error="handleError" @saved="onSetDefault" />
      </div>
      <TarTabs>
        <TarTab active id="general" :title="t('general')">
          <SenderGeneral :sender="sender" @error="handleError" @updated="onGeneralUpdated" />
        </TarTab>
        <TarTab id="settings" :title="t('settings.title')">
          <SendGridSettingsEdit v-if="sender.provider === 'SendGrid'" :sender="sender" @error="handleError" @updated="onSendGridUpdated" />
          <TwilioSettingsEdit v-if="sender.provider === 'Twilio'" :sender="sender" @error="handleError" @updated="onTwilioUpdated" />
        </TarTab>
        <TarTab id="demo" :title="t('messages.demo.label')">
          <SendDemo :sender="sender" />
        </TarTab>
      </TarTabs>
    </template>
  </main>
</template>
