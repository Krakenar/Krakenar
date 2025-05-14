<script setup lang="ts">
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import EmailIcon from "@/components/users/EmailIcon.vue";
import PhoneIcon from "@/components/users/PhoneIcon.vue";
import UserAvatar from "@/components/users/UserAvatar.vue";
import UserIcon from "@/components/users/UserIcon.vue";
import type { Message, Recipient } from "@/types/messages";
import { formatUser } from "@/helpers/format";

const { t } = useI18n();

const props = defineProps<{
  message: Message;
}>();

const recipients = computed<Recipient[]>(() => props.message.recipients);
</script>

<template>
  <div>
    <table class="table table-striped">
      <thead>
        <tr>
          <th scope="col">{{ t("messages.recipients.type.label") }}</th>
          <th scope="col"><EmailIcon /> {{ t("users.email.address") }}</th>
          <th scope="col"><PhoneIcon /> {{ t("users.phone.number") }}</th>
          <th scope="col">{{ t("displayName") }}</th>
          <th scope="col"><UserIcon /> {{ t("messages.recipients.user") }}</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="(recipient, index) in recipients" :key="index">
          <td>{{ t(`messages.recipients.type.options.${recipient.type}`) }}</td>
          <td>
            <template v-if="recipient.user?.email">{{ recipient.user.email.address }}</template>
            <template v-else-if="recipient.email">{{ recipient.email.address }}</template>
            <span v-else class="text-muted">—</span>
          </td>
          <td>
            <template v-if="recipient.user?.phone">{{ recipient.user.phone.e164Formatted }}</template>
            <template v-else-if="recipient.phone">{{ recipient.phone.e164Formatted }}</template>
            <span v-else class="text-muted">—</span>
          </td>
          <td>
            <template v-if="recipient.user?.fullName">{{ recipient.user.fullName }}</template>
            <template v-else-if="recipient.displayName">{{ recipient.displayName }}</template>
            <span v-else class="text-muted">—</span>
          </td>
          <td>
            <template v-if="recipient.user">
              <UserAvatar v-if="recipient.user.version > 0" target="_blank" :user="recipient.user" />
              <template v-else><font-awesome-icon icon="fas fa-user-slash" /> {{ formatUser(recipient.user) }}</template>
            </template>
            <span v-else class="text-muted">—</span>
          </td>
        </tr>
      </tbody>
    </table>
  </div>
</template>
