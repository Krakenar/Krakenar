<script setup lang="ts">
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import EnabledBadge from "./EnabledBadge.vue";
import StatusBlock from "@/components/shared/StatusBlock.vue";
import type { User } from "@/types/users";

const { d, t } = useI18n();

const props = defineProps<{
  user: User;
}>();

const addressLines = computed<string[]>(() => props.user.address?.formatted.split("\n") ?? []);
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
            <!-- TODO(fpion): (un)verified -->
          </template>
          <span class="text-muted" v-else>{{ "—" }}</span>
        </td>
      </tr>
      <tr>
        <th scope="row">{{ t("users.email.address") }}</th>
        <td>
          <template v-if="user.email">
            {{ user.email.address }}
            <!-- TODO(fpion): (un)verified -->
          </template>
          <span class="text-muted" v-else>{{ "—" }}</span>
        </td>
      </tr>
      <tr>
        <th scope="row">{{ t("users.phone.number") }}</th>
        <td>
          <template v-if="user.phone">
            {{ user.phone.e164Formatted }}
            <!-- TODO(fpion): (un)verified -->
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
