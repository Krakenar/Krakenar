<script setup lang="ts">
import { useI18n } from "vue-i18n";

import EnabledBadge from "./EnabledBadge.vue";
import StatusBlock from "@/components/shared/StatusBlock.vue";
import type { User } from "@/types/users";

const { d, t } = useI18n();

defineProps<{
  user: User;
}>();
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
          <p v-if="user.address">todo</p>
          <span class="text-muted" v-else>{{ "—" }}</span>
        </td>
      </tr>
      <tr>
        <th scope="row">{{ t("users.email.address") }}</th>
        <td>
          <p v-if="user.email">todo</p>
          <span class="text-muted" v-else>{{ "—" }}</span>
        </td>
      </tr>
      <tr>
        <th scope="row">{{ t("users.phone.number") }}</th>
        <td>
          <p v-if="user.phone">todo</p>
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
