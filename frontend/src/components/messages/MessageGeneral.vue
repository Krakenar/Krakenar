<script setup lang="ts">
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import DemoBadge from "./DemoBadge.vue";
import MessageResults from "./MessageResults.vue";
import SenderIcon from "@/components/senders/SenderIcon.vue";
import StatusBadge from "./StatusBadge.vue";
import type { Message } from "@/types/messages";
import type { Sender } from "@/types/senders";
import { formatSender } from "@/helpers/format";

const { t } = useI18n();

const props = defineProps<{
  message: Message;
}>();

const sender = computed<Sender>(() => props.message.sender);
</script>

<template>
  <div>
    <table class="table table-striped">
      <tbody>
        <tr>
          <th scope="row">{{ t("senders.select.label") }}</th>
          <td>
            <RouterLink v-if="sender.version > 0" :to="{ name: 'SenderEdit', params: { id: sender.id } }" target="_blank">
              <SenderIcon :sender="sender" /> {{ formatSender(message.sender) }}
            </RouterLink>
            <span v-else class="text-muted"><SenderIcon :sender="sender" /> {{ formatSender(message.sender) }}</span>
          </td>
        </tr>
        <tr>
          <th scope="row">{{ t("senders.provider.label") }}</th>
          <td>{{ t(`senders.provider.options.${message.sender.provider}`) }}</td>
        </tr>
        <tr>
          <th scope="row">{{ t("messages.status.label") }}</th>
          <td>
            <StatusBadge :message="message" />
          </td>
        </tr>
        <tr v-if="message.isDemo">
          <th scope="row">{{ t("messages.demo.label") }}</th>
          <td>
            <DemoBadge /> <i class="text-info">{{ t("messages.demo.hint") }}</i>
          </td>
        </tr>
      </tbody>
    </table>
    <MessageResults v-if="message.results.length > 0" :results="message.results" />
  </div>
</template>
