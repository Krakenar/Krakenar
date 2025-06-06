<script setup lang="ts">
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import EnabledBadge from "./EnabledBadge.vue";
import StatusBlock from "@/components/shared/StatusBlock.vue";
import StatusInfo from "@/components/shared/StatusInfo.vue";
import type { User } from "@/types/users";

const { d, t } = useI18n();

const props = defineProps<{
  user: User;
}>();

const addressLines = computed<string[]>(() => {
  const lines: string[] = props.user.address?.formatted.split("\n") ?? [];
  return [lines[0], lines.slice(1).join(" ")];
});
</script>

<template>
  <table class="table table-striped">
    <tbody>
      <tr>
        <th scope="row">{{ t("users.sort.options.FullName") }}</th>
        <td>
          <template v-if="user.fullName">{{ user.fullName }}</template>
          <span class="text-muted" v-else>{{ "—" }}</span>
        </td>
      </tr>
      <tr>
        <th scope="row">{{ t("users.address.title") }}</th>
        <td>
          <template v-if="addressLines.length > 0">
            <template v-for="(line, index) in addressLines" :key="index">
              {{ line }}
              <br v-if="index < addressLines.length - 1" />
            </template>
            <template v-if="user.address && user.address.verifiedBy && user.address.verifiedOn">
              <br />
              <font-awesome-icon class="me-1" icon="fas fa-certificate" />
              <StatusInfo :actor="user.address.verifiedBy" :date="user.address.verifiedOn" format="users.address.verified.format" />
            </template>
          </template>
          <span class="text-muted" v-else>{{ "—" }}</span>
        </td>
      </tr>
      <tr>
        <th scope="row">{{ t("users.email.address") }}</th>
        <td>
          <template v-if="user.email">
            {{ user.email.address }}
            <template v-if="user.email.verifiedBy && user.email.verifiedOn">
              <br />
              <font-awesome-icon class="me-1" icon="fas fa-certificate" />
              <StatusInfo :actor="user.email.verifiedBy" :date="user.email.verifiedOn" format="users.email.verified.format" />
            </template>
          </template>
          <span class="text-muted" v-else>{{ "—" }}</span>
        </td>
      </tr>
      <tr>
        <th scope="row">{{ t("users.phone.title") }}</th>
        <td>
          <template v-if="user.phone">
            {{ user.phone.e164Formatted }}
            <template v-if="user.phone.verifiedBy && user.phone.verifiedOn">
              <br />
              <font-awesome-icon class="me-1" icon="fas fa-certificate" />
              <StatusInfo :actor="user.phone.verifiedBy" :date="user.phone.verifiedOn" format="users.phone.verified.format" />
            </template>
          </template>
          <span class="text-muted" v-else>{{ "—" }}</span>
        </td>
      </tr>
      <tr>
        <th scope="row">{{ t("users.sort.options.AuthenticatedOn") }}</th>
        <td>
          <template v-if="user.authenticatedOn">
            {{ d(user.authenticatedOn, "medium") }}
            <br />
            <RouterLink :to="{ name: 'SessionList', query: { user: user.id } }" target="_blank">
              <font-awesome-icon icon="fas fa-user-clock" />
              {{ t("users.viewSessions") }}
            </RouterLink>
          </template>
          <span class="text-muted" v-else>{{ "—" }}</span>
        </td>
      </tr>
      <tr>
        <th scope="row">{{ t("users.sort.options.DisabledOn") }}</th>
        <td>
          <StatusBlock v-if="user.disabledBy && user.disabledOn" :actor="user.disabledBy" :date="user.disabledOn" format="users.disabledOn" />
          <EnabledBadge v-else />
        </td>
      </tr>
    </tbody>
  </table>
</template>
