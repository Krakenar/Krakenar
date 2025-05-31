<script setup lang="ts">
import { inject, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";

import ContentIcon from "@/components/contents/ContentIcon.vue";
import EditIcon from "@/components/shared/EditIcon.vue";
import MessageIcon from "@/components/messages/MessageIcon.vue";
import PersistentBadge from "@/components/sessions/PersistentBadge.vue";
import RealmIcon from "@/components/realms/RealmIcon.vue";
import SessionIcon from "@/components/sessions/SessionIcon.vue";
import StatusBlock from "@/components/shared/StatusBlock.vue";
import UserAvatar from "@/components/users/UserAvatar.vue";
import UserIcon from "@/components/users/UserIcon.vue";
import type { Session } from "@/types/sessions";
import type { Statistics } from "@/types/dashboard";
import { getStatistics } from "@/api/dashboard";
import { handleErrorKey } from "@/inject/App";
import { useRealmStore } from "@/stores/realm";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const realm = useRealmStore();
const { d, t } = useI18n();

const statistics = ref<Statistics>();

function getIpAddress(session: Session): string | undefined {
  return session.customAttributes.find(({ key }) => key === "IpAddress")?.value;
}

onMounted(async () => {
  try {
    statistics.value = await getStatistics();
  } catch (e: unknown) {
    handleError(e);
  }
});
</script>

<template>
  <main class="container">
    <template v-if="statistics">
      <h1>{{ t("dashboard") }}</h1>
      <div class="d-flex flex-column justify-content-center align-items-center mt-3">
        <div class="dash-grid-main">
          <RouterLink v-if="!realm.currentRealm" :to="{ name: 'RealmList' }" class="tile">
            <span><RealmIcon /> {{ t("realms.title") }}</span>
            <br />
            <span class="icon">{{ statistics.realmCount }}</span>
          </RouterLink>
          <RouterLink :to="{ name: 'UserList' }" class="tile">
            <span><UserIcon /> {{ t("users.title") }}</span>
            <br />
            <span class="icon">{{ statistics.userCount }}</span>
          </RouterLink>
          <RouterLink :to="{ name: 'SessionList' }" class="tile">
            <span><SessionIcon /> {{ t("sessions.title.list") }}</span>
            <br />
            <span class="icon">{{ statistics.sessionCount }}</span>
          </RouterLink>
          <RouterLink :to="{ name: 'MessageList' }" class="tile">
            <span><MessageIcon /> {{ t("messages.title") }}</span>
            <br />
            <span class="icon">{{ statistics.messageCount }}</span>
          </RouterLink>
          <RouterLink v-if="realm.currentRealm" :to="{ name: 'ContentList' }" class="tile">
            <span><ContentIcon /> {{ t("contents.item.title") }}</span>
            <br />
            <span class="icon">{{ statistics.contentCount }}</span>
          </RouterLink>
        </div>
      </div>
      <h5>{{ t("sessions.active.list") }}</h5>
      <table v-if="statistics.sessions.length" class="table table-striped">
        <thead>
          <tr>
            <th scope="col">{{ t("sessions.sort.options.CreatedOn") }}</th>
            <th scope="col">{{ t("users.select.label") }}</th>
            <th scope="col">{{ t("sessions.ipAddress") }}</th>
            <th scope="col">{{ t("sessions.sort.options.UpdatedOn") }}</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="session in statistics.sessions" :key="session.id">
            <td>
              <RouterLink :to="{ name: 'SessionEdit', params: { id: session.id } }"><EditIcon /> {{ d(session.createdOn, "medium") }}</RouterLink>
              <template v-if="session.isPersistent">
                <br />
                <PersistentBadge />
              </template>
            </td>
            <td>
              <UserAvatar target="_blank" :user="session.user" />
            </td>
            <td>
              <template v-if="getIpAddress(session)">{{ getIpAddress(session) }}</template>
              <span v-else class="text-muted">—</span>
            </td>
            <td><StatusBlock :actor="session.updatedBy" :date="session.updatedOn" /></td>
          </tr>
        </tbody>
      </table>
      <p v-else>{{ t("sessions.empty") }}</p>
    </template>
  </main>
</template>

<style scoped>
.dash-grid-main {
  display: grid;
  grid-template-columns: repeat(var(--columns), var(--column-width));
  gap: var(--gap);
  max-width: calc(var(--columns) * var(--column-width) + (var(--columns) - 1) * var(--gap));
  margin-bottom: var(--gap);
  --columns: 1;
  --gap: 1.5rem;
  --column-width: 13.5rem;
  --column-height: 13.5rem;
}

.tile {
  box-shadow: rgba(99, 99, 99, 0.2) 0px 2px 8px 0px;
  background-color: var(--bs-tertiary-bg);
  border: 1px solid var(--bs-border-color);
  border-radius: 0.75rem;
  width: 100%;
  max-width: var(--column-width);
  height: var(--column-height);
  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: center;
  font-size: 1.5rem;
  text-decoration: none;
}

.tile:hover {
  background-color: var(--bs-secondary-bg);
  cursor: pointer;
}

.tile .icon {
  font-size: 4.5rem;
}

@media (min-width: 576px) {
  .dash-grid-main {
    --columns: 2;
  }
}

@media (min-width: 768px) {
  .dash-grid-main {
    --columns: 3;
  }
}

@media (min-width: 992px) {
  .dash-grid-main {
    --columns: 4;
  }
}
</style>
