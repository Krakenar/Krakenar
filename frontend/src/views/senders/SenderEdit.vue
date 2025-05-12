<script setup lang="ts">
import { TarTab, TarTabs } from "logitar-vue3-ui";
import { inject, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import DeleteSender from "@/components/senders/DeleteSender.vue";
import SenderGeneral from "@/components/senders/SenderGeneral.vue";
import StatusDetail from "@/components/shared/StatusDetail.vue";
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
      <h1>{{ formatSender(sender) }}</h1>
      <StatusDetail :aggregate="sender" />
      <div class="mb-3">
        <DeleteSender :sender="sender" @deleted="onDeleted" @error="handleError" />
      </div>
      <TarTabs>
        <TarTab active id="general" :title="t('general')">
          <SenderGeneral :sender="sender" @error="handleError" @updated="onGeneralUpdated" />
        </TarTab>
      </TarTabs>
    </template>
  </main>
</template>
